using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using PizzaBot.Models;

namespace PizzaBot.Dialogs
{
    public class CustomerInfoDialog : ComponentDialog
    {
        private string name;
        private string phone;
        private string street;
        private string city;
        private string state;
        private string zip;
        public CustomerInfoDialog()
            : base(nameof(CustomerInfoDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerNameStepAsync,
                CustomerPhoneStepAsync,
                CustomerStreetStepAsync,
                CustomerCityStepAsync,
                CustomerStateStepAsync,
                CustomerZipStepAsync, 
                EndDialogStepAsync
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> CustomerNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Good choice! May I please have your name?")

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> CustomerPhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;
            name = (string)stepContext.Values["name"];

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("And a phone number to reach you if needed?")

            }, cancellationToken);
        }
        private async Task<DialogTurnResult> CustomerStreetStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["phone"] = (string)stepContext.Result;
            phone = (string)stepContext.Values["phone"];

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your street address.")

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> CustomerCityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["street"] = (string)stepContext.Result;
            street = (string)stepContext.Values["street"];

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your city.")

            }, cancellationToken);

        }

        private async Task<DialogTurnResult> CustomerStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["city"] = (string)stepContext.Result;
            city = (string)stepContext.Values["city"];

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your state.")

            }, cancellationToken);

        }

        private async Task<DialogTurnResult> CustomerZipStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["state"] = (string)stepContext.Result;
            state = (string)stepContext.Values["state"];

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("And your zip?")

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["zip"] = (string)stepContext.Result;
            zip = (string)stepContext.Values["zip"];

            Address newAddress = new Address(street, city, state, zip);
            Customer newCustomer = new Customer(name, phone, newAddress);
            Order newOrder = new Order();
            newOrder.Customer = newCustomer;
            
            return await stepContext.EndDialogAsync(newOrder, cancellationToken);
        }
    }
}
