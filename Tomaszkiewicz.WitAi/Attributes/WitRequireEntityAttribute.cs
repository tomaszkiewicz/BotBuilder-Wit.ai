using System;

namespace Tomaszkiewicz.WitAi.Attributes
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