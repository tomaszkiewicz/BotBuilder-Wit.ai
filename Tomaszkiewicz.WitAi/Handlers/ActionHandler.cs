using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi.Handlers
{
    public delegate Task<bool> ActionHandler(WitContext witContext, WitResult witResult, IWitPrivateConversationDataPersistence persistence);
}