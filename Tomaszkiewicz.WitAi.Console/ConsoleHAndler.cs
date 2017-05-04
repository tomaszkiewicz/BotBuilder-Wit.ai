using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.Console
{
    internal class ConsoleHandler : IDefaultIntentHandler
    {
        public Task Say(string text)
        {
            var baseColor = System.Console.ForegroundColor;

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine($"< {text}");
            System.Console.ForegroundColor = baseColor;
            
            return Task.CompletedTask;
        }

        public Task QuickReplies(string text, string[] quickReplies)
        {
            var baseColor = System.Console.ForegroundColor;

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine($"< {text}");           

            foreach (var quickReply in quickReplies)
                System.Console.WriteLine($"< * {quickReply}");

            System.Console.ForegroundColor = baseColor;

            return Task.CompletedTask;
        }

        [WitAction("reset")]
        [WitReset]
        public Task<bool> Reset(Dictionary<string, object> witContext, WitResult witResult, IWitPrivateConversationDataPersistence persistence)
        {
            return Task.FromResult(false);
        }
    }
}