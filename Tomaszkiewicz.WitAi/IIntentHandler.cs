using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi
{
    public interface IIntentHandler
    {
        Task Say(string text);
        Task QuickReplies(string text, string[] quickReplies);
    }
}