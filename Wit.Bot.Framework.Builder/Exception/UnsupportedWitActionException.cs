using System;
using System.Runtime.Serialization;

namespace Wit.Bot.Framework.Builder.Exception
{
    [Serializable]
    internal class UnsupportedWitActionException : System.Exception
    {
        public UnsupportedWitActionException()
        {
        }

        public UnsupportedWitActionException(string message) : base(message)
        {
        }

        public UnsupportedWitActionException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedWitActionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}