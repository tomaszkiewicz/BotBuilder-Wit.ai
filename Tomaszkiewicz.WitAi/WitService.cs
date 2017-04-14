using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tomaszkiewicz.WitAi
{
    [Serializable]
    public sealed class WitService
    {
        private readonly string _authToken;

        public WitService(string authToken)
        {
            if (authToken == null)
                throw new InvalidOperationException("authToken cannot be null");

            _authToken = authToken;
        }

        public async Task<WitResult> QueryAsync(string text, string sessionId, string context)
        {
            var request = BuildRequest(text, sessionId, context);

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();

                try
                {
                    var result = JsonConvert.DeserializeObject<WitResult>(json);

                    return result;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException("Unable to deserialize Wit response.", ex);
                }
            }
        }

        private HttpRequestMessage BuildRequest(string text, string sessionId, string context)
        {
            if (sessionId == null)
                throw new InvalidOperationException("sessionId cannot be ampty");

            var queryParameters = new List<string>
            {
                $"session_id={Uri.EscapeDataString(sessionId)}"
            };

            if (!string.IsNullOrWhiteSpace(text))
                queryParameters.Add($"q={Uri.EscapeDataString(text)}");

            var builder = new UriBuilder("https://api.wit.ai/converse?v=20160526")
            {
                Query = string.Join("&", queryParameters)
            };

            var request = new HttpRequestMessage()
            {
                RequestUri = builder.Uri,
                Method = HttpMethod.Post,
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            request.Content = new StringContent(context, Encoding.UTF8, "application/json");

            return request;
        }


    }
}