using System;

namespace Wit.Bot.Framework.Builder.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WitRequireEntityAttribute : Attribute
    {
        public string Name { get; set; }

        public WitRequireEntityAttribute(string name)
        {
            Name = name;
        }
    }
}