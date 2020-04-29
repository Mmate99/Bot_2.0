using CoreBot.Clause;
using CoreBot.CognitiveModels;
using CoreBot.Database;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.BotBuilderSamples.Dialogs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs {
    public class EditQueryDialog : ComponentDialog {
        private readonly LuisRecogniser _luisRecognizer;
        protected readonly ILogger Logger;
        private Dictionary<int, Query> queries;
        private Query selectedQuery;
        private int selectedClauseID;

        public EditQueryDialog(LuisRecogniser luisRecognizer, ILogger<EditQueryDialog> logger)
            : base(nameof(EditQueryDialog)) {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ResultStepAsync,
                PromptQueryIDAsync,
                PromptClauseToEditAsync,
                GetNewClauseAsync,
                ReplaceClauseAsync,
                PrintNewQueryAsync
                //EndStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            queries = MainDialog.getQueries();

            var messageText = "Do you want to edit one fo them?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            promptMessage.SuggestedActions = new SuggestedActions() {
                Actions = new List<CardAction>() {
                    new CardAction() { Title = "Yes", Type = ActionTypes.ImBack, Value = "Yes" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions() { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            string result = stepContext.Result.ToString();

            switch (result) {
                case "No":
                    return await stepContext.EndDialogAsync(null, cancellationToken);

                case "Yes":
                    return await stepContext.NextAsync();

                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please answer with a simple Yes or No."));
                    await stepContext.EndDialogAsync(null, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(EditQueryDialog), null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> PromptQueryIDAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {

            var messageText = "What is the ID of the query you want to change?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            List<CardAction> actions = new List<CardAction>();

            foreach (var q in queries) {
                actions.Add(new CardAction() { Type = ActionTypes.ImBack, Value = "Query ID: " + q.Key });
            }

            promptMessage.SuggestedActions = new SuggestedActions();
            promptMessage.SuggestedActions.Actions = actions;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> PromptClauseToEditAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            string result = stepContext.Result.ToString();
            int queryID = Convert.ToInt32(result.Split(" ")[2]);
            selectedQuery = queries[queryID];

            string txt = selectedQuery.getText() + Environment.NewLine;

            foreach (var cl in selectedQuery.getClauses()) {
                txt += "Clause ID: " + cl.Key + ", searchkey: " + cl.Value.SearchKey + ", value: " + cl.Value.Value + Environment.NewLine;
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(txt));

            var messageText = "What is the ID of the clause you want to change?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            List<CardAction> actions = new List<CardAction>();

            foreach (var cl in selectedQuery.getClauses()) {
                actions.Add(new CardAction() { Type = ActionTypes.ImBack, Value = "Clause ID: " + cl.Key });
            }

            promptMessage.SuggestedActions = new SuggestedActions();
            promptMessage.SuggestedActions.Actions = actions;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> GetNewClauseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            string result = stepContext.Result.ToString();
            selectedClauseID = Convert.ToInt32(result.Split(" ")[2]);
            LuisClause selectedClause = selectedQuery.getClauses()[selectedClauseID];

            var messageText = "Type in the new clause!";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ReplaceClauseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            var luisResult = await _luisRecognizer.RecognizeAsync<GeneralStatemenHandler>(stepContext.Context, cancellationToken);
            Dictionary<LuisClause, string> nc = new Dictionary<LuisClause, string>();

            ClauseHandler ch = new ClauseHandler(luisResult, nc);

            LuisClause newClause = nc.Last().Key;
            selectedQuery.getClauses()[selectedClauseID] = newClause;

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PrintNewQueryAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            string queryClauses = "";

            foreach (var cl in selectedQuery.getClauses()) {
                queryClauses += "Searchkey: " + cl.Value.SearchKey;
                queryClauses += cl.Value.Negated != false ? ", not equals" : "";
                queryClauses += cl.Value.Smaller != false ? ", smaller than" : "";
                queryClauses += cl.Value.Bigger != false ? ", bigger than" : "";
                queryClauses += ", value: " + cl.Value.Value + Environment.NewLine;
            }

            var msg = MessageFactory.Text("The new query:" + Environment.NewLine + queryClauses);
            await stepContext.Context.SendActivityAsync(msg);

            return await stepContext.NextAsync();
        }

        //Run new query? 
    }
}
