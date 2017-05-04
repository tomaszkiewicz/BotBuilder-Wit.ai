using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi
{
    public interface IWitPrivateConversationDataPersistence
    {
        Task<bool> TryGetPrivateConversationData(string name, out object data);
        Task SetPrivateConversationData(string name, object data);
    }
}