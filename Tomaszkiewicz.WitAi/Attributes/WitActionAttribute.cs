using System;

namespace Tomaszkiewicz.WitAi.Attributes
{
    /// <summary>
    /// This parameter specifies action name that gets invoked by wit.ai.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
    public class WitActionAttribute : Attribute
    {
        public string ActionName { get; set; }

        public WitActionAttribute(string actionName)
        {
            ActionName = actionName;
        }

    }
}