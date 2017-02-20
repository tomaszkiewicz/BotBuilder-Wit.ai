using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WitMergeAttribute : Attribute
    {
        public string Name { get; set; }
        
        public WitMergeAttribute(string name)
        {
            Name = name;
        }
    }
}