using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using PizzaBot.Models;
using System.Text;

namespace PizzaBot.Dialogs
{
    public class OrderDialog : ComponentDialog
    {
        //private readonly IStatePropertyAccessor<Order> _orderAccessor;
        private readonly Order order;
        public OrderDialog(UserState userState)
            : base(nameof(OrderDialog))
        {
            //_orderAccessor = userState.CreateProperty<Order>("Order");
            order = new Order();

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OrderTypeStepAsync,
                AddItemsStepAsync,
                CustomerInfoStepAsync,
                OrderConfirmStepAsync
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new AddItemsDialog());
            AddDialog(new CustomerInfoDialog());

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

        private static async Task<DialogTurnResult> AddItemsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["orderType"] = ((FoundChoice)stepContext.Result).Value; //TODO: will need to figure out how to store a List of items in the ConversationState
            return await stepContext.BeginDialogAsync(nameof(AddItemsDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> CustomerInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // get OrderItem list from previous step and add to order member var
            order.OrderItems = stepContext.Result as List<OrderItem>;  //?? new List<OrderItem>();
            return await stepContext.BeginDialogAsync(nameof(CustomerInfoDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> OrderConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var lastOrderItem = (string)stepContext.Result;
            var customer = (Customer)stepContext.Result;
            // Get the current profile object from user state.
            //var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);
            string orderType = (string)stepContext.Values["orderType"];
            order.Type = (Order.OrderType)Enum.Parse(typeof(Order.OrderType), orderType);
            order.Customer = customer;
            // order.OrderItems = new List<OrderItem>()

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {order.Customer.Name}! We will process your {order.Type} order. " +
                $"Here is your order summary: {PrintOrderSummary(order.OrderItems)}"), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private string PrintOrderSummary(List<OrderItem> items)
        {
            var summary = new StringBuilder();
            foreach(OrderItem item in items)
            {
                summary.Append(item.ToString()).Append("\n");
            }
            return summary.ToString();
        }
    }
}
