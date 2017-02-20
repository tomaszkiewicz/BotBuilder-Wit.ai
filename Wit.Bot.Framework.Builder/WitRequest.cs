using System;
using System.Collections.Generic;
using Microsoft.Rest;
using Wit.Bot.Framework.Builder.Interfaces;

namespace Wit.Bot.Framework.Builder
{
    public sealed class WitRequest
    {
        public string Query { get; }
        public string SessionId { get; }
        public string Context { get; }

        public WitRequest(string query, string sessionId, string context = default(string))
        {
            Query = query;
            SessionId = sessionId;
            Context = context;
        }

        /// <summary>
        /// Build the Uri for issuing the request for the specified wit model.
        /// </summary>
        /// <param name="model"> The wit model.</param>
        /// <returns> The request Uri.</returns>
        public Uri BuildUri(IWitModel model)
        {
            if (SessionId == null)
                throw new ValidationException(ValidationRules.CannotBeNull, "session id");

            var queryParameters = new List<string>
            {
                $"session_id={Uri.EscapeDataString(SessionId)}",
                $"q={Uri.EscapeDataString(Query)}"
            };

            UriBuilder builder;

            switch (model.ApiVersion)
            {
                case WitApiVersion.Standard:
                    builder = new UriBuilder(model.UriBase);
                    break;

                default:
                    throw new ArgumentException($"{model.ApiVersion} is not a valid Wit api version.");
            }

            builder.Query = string.Join("&", queryParameters);

            return builder.Uri;
        }
    }
}