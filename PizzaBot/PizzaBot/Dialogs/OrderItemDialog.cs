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
        private Pizza pizza; // right now only branching to order a pizza 
        //(So this would need to be in a PizzaDialog or have mult fields for side and drink
        Order order;
        public OrderItemDialog()
            : base(nameof(OrderItemDialog))
        {
            order = new Order();

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DisplayItemMenuStepAsync,
                OrderItemStepAsync,
                CustomerInfoStepAsync, 
                EndDialogStepAsync
            }));

            AddDialog(new PizzaDialog());
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
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Pizza", "Side", "Drink" }),
                }, cancellationToken);
        }

        

       private async Task<DialogTurnResult> OrderItemStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var item = stepContext.Values["lastOrderItem"] = ((FoundChoice)stepContext.Result).Value; // keeping these k-v pairs in case I need them
            //var item = (OrderItem)stepContext.Values["lastOrderItem"];
            //switch (nameof(item))
            //{
            //    case "Pizza":
            //        return await stepContext.BeginDialogAsync(nameof(PizzaDialog), null, cancellationToken);
            //    case "SideItem":

            //}
            return (item) switch
            {
                "Pizza" => await stepContext.BeginDialogAsync(nameof(PizzaDialog), null, cancellationToken), 
                "Side" => await stepContext.BeginDialogAsync(nameof(PizzaDialog), null, cancellationToken),
                "Drink" => await stepContext.BeginDialogAsync(nameof(PizzaDialog), null, cancellationToken),
                _ => await stepContext.EndDialogAsync(null, cancellationToken) //TODO: Would want to handle invalid response, maybe add another layer on top of adapter
            };
        }

       
        private async Task<DialogTurnResult> CustomerInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: keep adding items here
            var item = (OrderItem)stepContext.Result;
            order.OrderItems.Add(item);

            // Then would call this when done adding items
            return await stepContext.BeginDialogAsync(nameof(CustomerInfoDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var order = (Order)stepContext.Result;
            var customer = (Customer)stepContext.Result;
            order.Customer = customer;

            return await stepContext.EndDialogAsync(order, cancellationToken);
        }
    }
}
