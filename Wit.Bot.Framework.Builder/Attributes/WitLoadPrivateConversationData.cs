using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    /// <summary>
    /// When this parameter is applied to the method the entity with name from argument is loaded from PrivateConversationData into context, but only id there is no such entity in context already.
    /// Loading takes place after all merges and before WitRequire checking.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WitLoadPrivateConversationData : Attribute
    {
        public string Name { get; set; }

        public WitLoadPrivateConversationData(string name)
        {
            Name = name;
        }
    }
}