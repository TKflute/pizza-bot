using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using PizzaBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaBot.Dialogs
{
    public class SideItemDialog : ComponentDialog
    {
        private SideItem sideItem;
        public SideItemDialog()
            : base(nameof(SideItemDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SideChoiceStepAsync,
                SideQtyStepAsync,
                EndDialogStepAsync
            }));

            // AddDialog(new CustomerInfoDialog());
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SideChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            sideItem = new SideItem
            {
                Name = "side"
            };

            string[] choices = Enum.GetNames(typeof(SideItem.ItemType));

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please choose a side below:"),
                    Choices = ChoiceFactory.ToChoices(new List<string> (choices)),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> SideQtyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = ((FoundChoice)stepContext.Result).Value;
            sideItem.Type = (SideItem.ItemType)Enum.Parse(typeof(SideItem.ItemType), choice);

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many orders of {choice}?")

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["qty"] = stepContext.Result;
            sideItem.Qty = Convert.ToInt32(stepContext.Values["qty"]);

            return await stepContext.EndDialogAsync(sideItem, cancellationToken);
        }
    }
}
