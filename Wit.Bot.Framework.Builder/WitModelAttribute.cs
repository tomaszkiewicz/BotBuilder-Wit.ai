using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Internals.Fibers;
using Wit.Bot.Framework.Builder.Interfaces;

namespace Wit.Bot.Framework.Builder
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    [Serializable]
    public class WitModelAttribute : Attribute, IWitModel, IEquatable<IWitModel>
    {
        public string AuthToken => _authToken;
        public Uri UriBase { get; }
        public WitApiVersion ApiVersion { get; }
        private readonly string _authToken;

        public static readonly IReadOnlyDictionary<WitApiVersion, Uri> WitEndpoints = new Dictionary<WitApiVersion, Uri>
        {
            {WitApiVersion.Standard, new Uri("https://api.wit.ai/converse?v=20160526")}
        };

        /// <summary>
        /// Construct the wit model information.
        /// </summary>
        /// <param name="authToken">The auth token for wit</param>
        /// <param name="apiVersion">The wit API version.</param>
        public WitModelAttribute(string authToken, WitApiVersion apiVersion = WitApiVersion.Standard)
        {
            SetField.NotNull(out _authToken, nameof(authToken), authToken);
            ApiVersion = apiVersion;
            UriBase = WitEndpoints[apiVersion];
        }

        public bool Equals(IWitModel other)
        {
            return other != null && Equals(AuthToken, other.AuthToken) && Equals(ApiVersion, other.ApiVersion) && Equals(UriBase, other.UriBase);
        }

        public override bool Equals(object other)
        {
            return Equals(other as IWitModel);
        }

        public override int GetHashCode()
        {
            return AuthToken.GetHashCode() ^ UriBase.GetHashCode() ^ ApiVersion.GetHashCode();
        }
    }
}
