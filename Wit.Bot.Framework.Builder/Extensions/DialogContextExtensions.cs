using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Wit.Bot.Framework.Builder.Extensions
{
    public static class DialogContextExtensions
    {
        public static async Task PostQuickRepliesAsync(this IDialogContext context, IEnumerable<string> quickReplies, string title = null, string subtitle = null, string message = null)
        {
            var menu = context.MakeMessage();

            menu.Attachments = new List<Attachment>();

            var card = new HeroCard
            {
                Text = message,
                Buttons = new List<CardAction>()
            };

            menu.Attachments.Add(card.ToAttachment());

            foreach (var quickReply in quickReplies)
            {
                var buttonTitle = quickReply;
                var buttonValue = quickReply;

                if (quickReply.Contains("|"))
                {
                    var index = quickReply.IndexOf("|", StringComparison.Ordinal);

                    buttonTitle = quickReply.Substring(0, index);
                    buttonValue = quickReply.Substring(index + 1);
                }

                var button = new CardAction
                {
                    Type = "imBack",
                    Title = buttonTitle,
                    Value = buttonValue
                };

                card.Buttons.Add(button);
            }

            await context.PostAsync(menu);
        }
    }
}