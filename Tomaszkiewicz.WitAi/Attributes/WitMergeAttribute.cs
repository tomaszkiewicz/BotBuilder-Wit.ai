using System;

namespace Tomaszkiewicz.WitAi.Attributes
{
    /// <summary>
    /// When this parameter is applied to the method the entity with name from argument is copied from wit result to context.
    /// </summary>
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