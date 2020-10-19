using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using PizzaBot.Models;

namespace PizzaBot.Dialogs
{
    public class OrderDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<Order> _orderAccessor;
        public OrderDialog(UserState userState)
            : base(nameof(OrderDialog))
        {
            _orderAccessor = userState.CreateProperty<Order>("Order");

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OrderTypeStepAsync,
                OrderItemsStepAsync,
                OrderConfirmStepAsync
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new OrderItemDialog());

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> OrderTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Will your order be for pickup or delivery today?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Pickup", "Delivery" }),
                }, cancellationToken);

        }

        private static async Task<DialogTurnResult> OrderItemsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["orderType"] = ((FoundChoice)stepContext.Result).Value; //TODO: will need to figure out how to store a List of items in the ConversationState
            return await stepContext.BeginDialogAsync(nameof(OrderItemDialog), null, cancellationToken);
        }


        private async Task<DialogTurnResult> OrderConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var lastOrderItem = (string)stepContext.Result;
            var order = (Order)stepContext.Result;
            // Get the current profile object from user state.
            //var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);
            string orderType = (string)stepContext.Values["orderType"];
            order.Type = (Order.OrderType)Enum.Parse(typeof(Order.OrderType), orderType);
            // order.OrderItems = new List<OrderItem>()

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {order.Customer.Name}! We will process your {order.OrderItems} order for {order.Type}"), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
