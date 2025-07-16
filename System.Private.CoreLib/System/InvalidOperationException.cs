namespace System
{
    public sealed class InvalidOperationException : Exception
    {
        const string DefaultMessage = "Operation is not valid due to the current state of the object.";
        public InvalidOperationException() : base(DefaultMessage)
        { 
        }

        public InvalidOperationException(string message) : base(message ?? DefaultMessage)
        {
        }
    }
}
