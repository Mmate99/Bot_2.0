using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
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

        public EditQueryDialog(LuisRecogniser luisRecognizer, ILogger<EditQueryDialog> logger)
            : base(nameof(EditQueryDialog)) {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            //AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                //SpecifyStepAsync,
                //EndStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            var messageText = stepContext.Options?.ToString() ?? "Do you want to edit one fo them?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            promptMessage.SuggestedActions = new SuggestedActions() {
                Actions = new List<CardAction>() {
                    new CardAction() { Title = "Yes", Type = ActionTypes.ImBack, Value = "Yes" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }
    }
}
