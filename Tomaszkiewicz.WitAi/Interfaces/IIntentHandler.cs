namespace Tomaszkiewicz.WitAi.Interfaces
{
    public interface IIntentHandler : IDefaultIntentHandler
    {
        string Intent { get; }
    }
}