using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    /// <summary>
    /// When this parameter is applied to the method all entities (except for intent) are copied from wit result to context.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method)]
    public class WitMergeAllAttribute : Attribute
    {
        
    }
}