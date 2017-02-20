using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WitEntityAttribute : Attribute
    {
        public string Name { get; set; }

        public WitEntityAttribute(string name)
        {
            Name = name;
        }
    }
}