using System;
using System.Runtime.Serialization;

namespace Wit.Bot.Framework.Builder.Exception
{
    [Serializable]
    internal class WitModelDisambiguationException : System.Exception
    {
        public WitModelDisambiguationException()
        {
        }

        public WitModelDisambiguationException(string message) : base(message)
        {
        }

        public WitModelDisambiguationException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected WitModelDisambiguationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}