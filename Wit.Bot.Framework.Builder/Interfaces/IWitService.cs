using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder.Interfaces
{
    public interface IWitService
    {
        HttpRequestMessage BuildRequest(WitRequest witRequest);

        Task<WitResult> QueryAsync(HttpRequestMessage request, CancellationToken token);
    }
}