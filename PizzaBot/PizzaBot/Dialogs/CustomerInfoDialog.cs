using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace PizzaBot.Dialogs
{
    public class CustomerInfoDialog : ComponentDialog
    {
        public CustomerInfoDialog()
            : base(nameof(CustomerInfoDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerStreetStepAsync,
                CustomerCityStepAsync,
                CustomerStateStepAsync,
                CustomerZipStepAsync, 
                EndDialogStepAsync
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> CustomerStreetStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Good choice! Please enter your street address.")

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> CustomerCityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["street"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your city.")

            }, cancellationToken);

        }

        private async Task<DialogTurnResult> CustomerStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["city"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your state.")

            }, cancellationToken);

        }

        private async Task<DialogTurnResult> CustomerZipStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["state"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("And your zip?")

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["zip"] = (string)stepContext.Result;

            return await stepContext.EndDialogAsync(/*stepContext.Values["zip"]*/null, cancellationToken);
        }
    }
}
