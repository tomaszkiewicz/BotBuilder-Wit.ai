using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WitMerge : Attribute
    {
        public string Name { get; set; }

        public WitMerge()
        {
            
        }

        public WitMerge(string name)
        {
            Name = name;
        }
    }
}