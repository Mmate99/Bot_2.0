// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CoreBot;
using CoreBot.Clause;
using CoreBot.Database;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog {
        private readonly LuisRecogniser _luisRecognizer;
        protected readonly ILogger Logger;
        private readonly Database database;
        private Dictionary<LuisClause, string> clauses = new Dictionary<LuisClause, string>();

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(LuisRecogniser luisRecognizer, ILogger<MainDialog> logger)
            : base(nameof(MainDialog)) {
            _luisRecognizer = luisRecognizer;
            Logger = logger;
            database = Program.database;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private bool findData(IData d) {
            bool ret = false;
            bool found = false;

            foreach (KeyValuePair<LuisClause, string> pair in clauses) {
                var prop = d.GetType().GetProperty(pair.Key.SearchKey, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                found = prop != null;

                if (pair.Key.Smaller) found = found && Convert.ToDouble(prop.GetValue(d)) < double.Parse(pair.Key.Value);

                else if (pair.Key.Bigger) found = found && Convert.ToDouble(prop.GetValue(d)) > double.Parse(pair.Key.Value);

                else if (pair.Key.Negated) found = found && prop.GetValue(d).ToString().ToUpper() != pair.Key.Value.ToUpper();

                else found = found && prop.GetValue(d).ToString().ToUpper() == pair.Key.Value.ToUpper();

                ret = pair.Value.Equals("and") ? ret && found : ret || found;
            }

            return ret;
        }

        private async Task handleResultsAsync(List<IData> res, WaterfallStepContext stepContext) {
            if (res.Count == 0) {
                string propNotFound = "Can't find any records with";

                foreach (KeyValuePair<LuisClause, string> pair in clauses) {
                    propNotFound += " property: " + pair.Key.SearchKey + ", value: " + pair.Key.Value + " and";
                }

                Regex regex = new Regex("(\\s+(and)\\s*)$");
                propNotFound = regex.Replace(propNotFound, "");

                var pnf = MessageFactory.Text(propNotFound);
                await stepContext.Context.SendActivityAsync(pnf);
                return;
            }

            foreach (var data in res) {
                string Class = data.ToString().Remove(0, 17);
                string Props = "";

                string type = "Class: " + Class + Environment.NewLine;
                string propsWithValues = "";

                var props = data.GetType().GetProperties();
                foreach (var p in props) {
                    propsWithValues += p.Name + ": " + p.GetValue(data).ToString() + Environment.NewLine;
                    Props += p.GetValue(data).ToString() + ",";
                }

                Props = Props.Remove(Props.Length - 1);

                string txt = type + propsWithValues;
                var msg = MessageFactory.Text(txt);
                await stepContext.Context.SendActivityAsync(msg);
            }
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageText = stepContext.Options?.ToString() ?? "Give me 'something is someting' sentences!";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<GeneralStatemenHandler>(stepContext.Context, cancellationToken);

            switch (luisResult.TopIntent().intent) {

                case GeneralStatemenHandler.Intent.Statement:

                    if (luisResult.Entities.Value[0] != null) {

                        ClauseHandler ch = new ClauseHandler(luisResult, clauses);

                        List<IData> list = database.getData();
                        List<IData> res = list;

                        List<IData> foundItems = list.FindAll(findData);
                        res = res.Intersect(foundItems).ToList();
                        await handleResultsAsync(res, stepContext);   
                    }

                    break;

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Restart the main dialog with a different message the second time around
            var promptMessage = "Are you looking for something else too?";
            //query ment�se....
            clauses = new Dictionary<LuisClause, string>();
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
