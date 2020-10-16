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
    public class OrderDialog : ComponentDialog
    {
        // TODO: Some of methods from sample project in this class are static and some are not. Why??
        private readonly IStatePropertyAccessor<Order> _orderAccessor;

        public OrderDialog(UserState userState)
            : base(nameof(OrderDialog))
        {

            _orderAccessor = userState.CreateProperty<Order>("Order");

            // This array defines how the Waterfall will execute.
            // TODO NEXT: Start by getting two simple steps to work, i.e. a Greeting step and Ordering one drink
            var waterfallSteps = new WaterfallStep[]
            {
                WelcomeStepAsync,
                NameStepAsync
                //NameConfirmStepAsync,
                //AgeStepAsync,
                //PictureStepAsync,
                //ConfirmStepAsync,
                //SummaryStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt), PicturePromptValidatorAsync));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> WelcomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Will your order be for pickup or delivery today?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Pickup", "Delivery"}),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //stepContext.Values["transport"] = ((FoundChoice)stepContext.Result).Value;

            //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            
            //**** My code starts here *****/
            stepContext.Values["orderType"] = ((FoundChoice)stepContext.Result).Value;

            // Get the current profile object from user state.
            var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);
            string orderType = (string)stepContext.Values["orderType"];
            order.Type = (Order.OrderType)Enum.Parse(typeof(Order.OrderType), orderType);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks! We will process your order for {order.Type}."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // User said "yes" so we will be prompting for the age.
                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please enter your age."),
                    RetryPrompt = MessageFactory.Text("The value entered must be greater than 0 and less than 150."),
                };

                return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);
            }
            else
            {
                // User said "no" so we will skip the next step. Give -1 as the age.
                return await stepContext.NextAsync(-1, cancellationToken);
            }
        }

        private static async Task<DialogTurnResult> PictureStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["age"] = (int)stepContext.Result;

            var msg = (int)stepContext.Values["age"] == -1 ? "No age given." : $"I have your age as {stepContext.Values["age"]}.";

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            if (stepContext.Context.Activity.ChannelId == Channels.Msteams)
            {
                // This attachment prompt example is not designed to work for Teams attachments, so skip it in this case
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Skipping attachment prompt in Teams channel..."), cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }
            else
            {
                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please attach a profile picture (or type any message to skip)."),
                    RetryPrompt = MessageFactory.Text("The attachment must be a jpeg/png image file."),
                };

                return await stepContext.PromptAsync(nameof(AttachmentPrompt), promptOptions, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["picture"] = ((IList<Attachment>)stepContext.Result)?.FirstOrDefault();

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Is this ok?") }, cancellationToken);
        }

        //private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    if ((bool)stepContext.Result)
        //    {
        //        // Get the current profile object from user state.
        //        var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

        //        order.Transport = (string)stepContext.Values["transport"];
        //        order.Name = (string)stepContext.Values["name"];
        //        order.Age = (int)stepContext.Values["age"];
        //        order.Picture = (Attachment)stepContext.Values["picture"];

        //        var msg = $"I have your mode of transport as {order.Transport} and your name as {order.Name}";

        //        if (order.Age != -1)
        //        {
        //            msg += $" and your age as {order.Age}";
        //        }

        //        msg += ".";

        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

        //        if (order.Picture != null)
        //        {
        //            try
        //            {
        //                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(order.Picture, "This is your profile picture."), cancellationToken);
        //            }
        //            catch
        //            {
        //                await stepContext.Context.SendActivityAsync(MessageFactory.Text("A profile picture was saved but could not be displayed here."), cancellationToken);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thanks. Your profile will not be kept."), cancellationToken);
        //    }

        //    // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
        //    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        //}

        private static Task<bool> AgePromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0 && promptContext.Recognized.Value < 150);
        }

        private static async Task<bool> PicturePromptValidatorAsync(PromptValidatorContext<IList<Attachment>> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Succeeded)
            {
                var attachments = promptContext.Recognized.Value;
                var validImages = new List<Attachment>();

                foreach (var attachment in attachments)
                {
                    if (attachment.ContentType == "image/jpeg" || attachment.ContentType == "image/png")
                    {
                        validImages.Add(attachment);
                    }
                }

                promptContext.Recognized.Value = validImages;

                // If none of the attachments are valid images, the retry prompt should be sent.
                return validImages.Any();
            }
            else
            {
                await promptContext.Context.SendActivityAsync("No attachments received. Proceeding without a profile picture...");

                // We can return true from a validator function even if Recognized.Succeeded is false.
                return true;
            }
        }
    }
}
