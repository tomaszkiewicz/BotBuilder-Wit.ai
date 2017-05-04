using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi
{
    public interface IWitPersistence : IWitPrivateConversationDataPersistence
    {
        Task<WitContext> GetWitContext();
        Task<string> GetSessionId();
        Task SetIntent(string intent);
        Task<string> GetIntent();
        Task<bool> GetResetRequested();
        Task SetResetRequested(bool resetRequested);
        Task SetSessionId(string sessionId);
        Task SetWitContext(WitContext witContext);
    }
}