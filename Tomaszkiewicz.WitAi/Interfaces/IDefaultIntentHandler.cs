using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi.Interfaces
{
    public interface IDefaultIntentHandler
    {
        Task Say(string text);
        Task QuickReplies(string text, string[] quickReplies);
    }
}