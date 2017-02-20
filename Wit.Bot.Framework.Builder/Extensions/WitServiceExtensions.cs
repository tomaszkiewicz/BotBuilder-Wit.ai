using System.Threading;
using System.Threading.Tasks;
using Wit.Bot.Framework.Builder.Interfaces;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder.Extensions
{
    public static class WitServiceExtensions
    {
        public static async Task<WitResult> QueryAsync(this IWitService service, string text, string sessionId, string context, CancellationToken token)
        {
            var request = service.BuildRequest(new WitRequest(text, sessionId, context));

            return await service.QueryAsync(request, token);
        }
    }
}