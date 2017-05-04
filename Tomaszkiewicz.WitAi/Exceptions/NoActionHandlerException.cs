namespace Tomaszkiewicz.WitAi.Exceptions
{
    public class NoActionHandlerException : WitDispatcherException
    {
        public NoActionHandlerException()
        {

        }

        public NoActionHandlerException(string message) : base(message)
        {

        }
    }
}