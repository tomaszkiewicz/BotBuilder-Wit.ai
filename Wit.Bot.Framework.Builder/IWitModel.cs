using System;

namespace Wit.Bot.Framework.Builder
{
    public interface IWitModel
    {
        string AuthToken { get; }
        
        Uri UriBase { get; }

        WitApiVersion ApiVersion { get; }
    }
}