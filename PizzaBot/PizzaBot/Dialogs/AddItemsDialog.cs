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
    public class AddItemsDialog : ComponentDialog
    {
        List<OrderItem> items;
        public AddItemsDialog()
            : base(nameof(AddItemsDialog))
        {
           items = new List<OrderItem>();

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DisplayItemMenuStepAsync,
                AddItemStepAsync,
                AddAnotherItemStepAsync,
                RestartOrEndStepAsync
            }));

            AddDialog(new PizzaDialog());
            AddDialog(new SideItemDialog());
            AddDialog(new DrinkDialog());
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

       private async Task<DialogTurnResult> AddItemStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
                "Side" => await stepContext.BeginDialogAsync(nameof(SideItemDialog), null, cancellationToken),
                "Drink" => await stepContext.BeginDialogAsync(nameof(DrinkDialog), null, cancellationToken),
                _ => await stepContext.EndDialogAsync(null, cancellationToken) //TODO: Would want to handle invalid response, maybe add another layer on top of adapter
            };
        }

       
        private async Task<DialogTurnResult> AddAnotherItemStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: keep adding items here
            var item = (OrderItem)stepContext.Result;
            items.Add(item);

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Would you like to add another item to your order?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No"}),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> RestartOrEndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (((FoundChoice)stepContext.Result).Value == "Yes")
            {
                return await stepContext.ReplaceDialogAsync(nameof(AddItemsDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(items, cancellationToken);
            }
        }
    }
}
