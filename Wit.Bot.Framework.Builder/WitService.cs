using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.Bot.Framework.Builder.Interfaces;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder
{
    [Serializable]
    public sealed class WitService : IWitService
    {
        private readonly IWitModel _model;

        /// <summary>
        /// Construct the wit service using the model information.
        /// </summary>
        /// <param name="model">The wit model information.</param>
        public WitService(IWitModel model)
        {
            SetField.NotNull(out _model, nameof(model), model);
        }

        public HttpRequestMessage BuildRequest(WitRequest witRequest)
        {
            var uri = witRequest.BuildUri(_model);

            return BuildRequest(uri, witRequest);
        }

        private HttpRequestMessage BuildRequest(Uri uri, WitRequest witRequest)
        {
            if (_model.AuthToken == null)
                throw new ValidationException(ValidationRules.CannotBeNull, "Authorization Token");

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _model.AuthToken);
            request.Content = new StringContent(witRequest.Context, Encoding.UTF8, "application/json");

            return request;
        }

        public async Task<WitResult> QueryAsync(HttpRequestMessage request, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request, token);

                var json = await response.Content.ReadAsStringAsync();
                
                try
                {
                    var result = JsonConvert.DeserializeObject<WitResult>(json);
                    //might need to add wiring based on action type here?

                    return result;
                }
                catch (JsonException ex)
                {
                    throw new ArgumentException("Unable to deserialize the Wit response", ex);
                }
            }
        }
    }
}