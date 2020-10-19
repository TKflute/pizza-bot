using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using PizzaBot.Models;

namespace PizzaBot.Dialogs
{
    public class OrderItemDialog : ComponentDialog
    {
        public OrderItemDialog()
            : base(nameof(OrderItemDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DisplayItemMenuStepAsync,
                PizzaSizeStepAsync,
                PizzaCrustStepAsync,
                PizzaToppingsStepAsync, 
                CustomerInfoStepAsync, 
                EndDialogStepAsync
            }));

            AddDialog(new CustomerInfoDialog());
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> DisplayItemMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What would you like to add to your order?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Pizza", "Sides", "Drinks" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> PizzaSizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["lastOrderItem"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What size pizza would you like?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Small", "Medium", "Large" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> PizzaCrustStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["pizzaSize"] = ((FoundChoice)stepContext.Result).Value; //TODO: will need to figure out how to store a List of items in the ConversationState

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What kind of crust?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Thin", "HandTossed", "DeepDish" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> PizzaToppingsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["pizzaCrust"] = ((FoundChoice)stepContext.Result).Value; //TODO: will need to figure out how to store a List of items in the ConversationState

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What kind of topping? (Choose one)"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Sausage", "Pepperoni", "Mushroom" }), //TODO: Select multiple toppings
                }, cancellationToken);

            // return await stepContext.BeginDialogAsync(nameof(OrderItemDialog), null, cancellationToken); // is this how you would restart a dialog (but with a Restart method)
          
        }

        private async Task<DialogTurnResult> CustomerInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["pizzaTopping"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.BeginDialogAsync(nameof(CustomerInfoDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var order = (Order)stepContext.Result;

            // TODO: start here: would need to save user input as a Pizza object as the conversation goes
            //order.OrderItems.Add((OrderItem)stepContext.Values["lastOrderItem"]);
          
            return await stepContext.EndDialogAsync(order, cancellationToken);
        }
    }
}
