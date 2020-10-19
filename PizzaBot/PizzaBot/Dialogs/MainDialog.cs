using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using PizzaBot.Models;


namespace PizzaBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        // TODO: Some of methods from sample project in this class are static and some are not. Why??
        private readonly UserState _userState;

        public MainDialog(UserState userState)
            : base(nameof(MainDialog))
        {

            _userState = userState;

            AddDialog(new OrderDialog(_userState));

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                WelcomeStepAsync,
                GoodbyeStepAsync
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
           
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
           

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> WelcomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = "Hello and welcome to Pizza Bot!";

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(OrderDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> GoodbyeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = "Thanks for your order, and enjoy your pizza!";

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }  
    }
}
