using System.Collections.Generic;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Attributes;

namespace Tomaszkiewicz.WitAi.Console
{
    internal abstract class ConsoleHandler : IIntentHandler
    {
        public Task Say(string text)
        {
            System.Console.WriteLine($"< {text}");

            return Task.CompletedTask;
        }

        public Task QuickReplies(string text, string[] quickReplies)
        {
            System.Console.WriteLine($"< {text}");

            foreach (var quickReply in quickReplies)
                System.Console.WriteLine($"< * {quickReply}");

            return Task.CompletedTask;
        }

        [WitAction("reset")]
        [WitReset]
        public Task Reset(Dictionary<string, object> witContext, WitResult witResult, ref bool requestReset)
        {
            return Task.CompletedTask;
        }
    }
}