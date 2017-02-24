using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    /// <summary>
    /// Resets the context after execution of the method marked with this attribute.
    /// To be more precise - the context is reset after receiving stop from wit.ai, so there may be some additional method invocations before reset, depending on wit.ai story construction.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method)]
    public class WitResetAttribute : Attribute
    {
        
    }
}