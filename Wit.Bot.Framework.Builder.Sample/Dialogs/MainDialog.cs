using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Wit.Bot.Framework.Builder.Attributes;
using Wit.Bot.Framework.Builder.Dialogs;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder.Sample.Dialogs
{
    [Serializable]
    [WitModel("VZQQJVTB2E5DBMXL2RXSS73CX75H4ABO")]
    public class MainDialog : WitDialog<object>
    {
        [WitAction("greetings")]
        public async Task Greetings(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            var username = (await message).From.Name;

            WitContext["user"] = username;

            if (!context.PrivateConversationData.ContainsKey("user"))
                context.PrivateConversationData.SetValue("user", username);
        }

        [WitAction("getForecast")]
        [WitRequireEntity("location")]
        [WitLoadPrivateConversationData("location")]
        [WitRequireEntity("datetime")]
        [WitMergeAll]
        public Task GetForecast(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            if(!context.PrivateConversationData.ContainsKey("location"))
                context.PrivateConversationData.SetValue("location", WitContext["location"]);

            WitContext["forecast"] = "sunny";

            return Task.CompletedTask;
        }

        [WitAction("reset")]
        [WitReset]
        public Task Reset(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            return Task.CompletedTask;
        }
    }
}