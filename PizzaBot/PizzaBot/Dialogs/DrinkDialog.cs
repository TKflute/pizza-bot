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
    public class DrinkDialog : ComponentDialog
    {
        private Drink drink;
        public DrinkDialog()
            : base(nameof(DrinkDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DrinkChoiceStepAsync, 
                DrinkQtyStepAsync, // this step is dup code - just getting the conversation flow for now
                EndDialogStepAsync
            }));

            // AddDialog(new CustomerInfoDialog());
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DrinkChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            drink = new Drink
            {
                Name = "drink"
            };

            string[] choices = Enum.GetNames(typeof(Drink.SodaType));

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please choose a soda below:"),
                    Choices = ChoiceFactory.ToChoices(new List<string>(choices)),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> DrinkQtyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = ((FoundChoice)stepContext.Result).Value;
            drink.Type = (Drink.SodaType)Enum.Parse(typeof(Drink.SodaType), choice);

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions
            {
                Prompt = MessageFactory.Text($"How many liters of {choice}?") // only diff here is "liters" vs "orders"

            }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["qty"] = stepContext.Result;
            drink.Qty = Convert.ToInt32(stepContext.Values["qty"]);

            return await stepContext.EndDialogAsync(drink, cancellationToken);
        }

    }
}
