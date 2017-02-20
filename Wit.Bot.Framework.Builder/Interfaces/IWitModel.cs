using System;

namespace Wit.Bot.Framework.Builder.Interfaces
{
    public interface IWitModel
    {
        string AuthToken { get; }
        Uri UriBase { get; }
        WitApiVersion ApiVersion { get; }
    }
}