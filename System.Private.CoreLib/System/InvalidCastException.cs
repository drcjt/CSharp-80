namespace System
{
    public sealed class InvalidCastException : Exception
    {
        const string DefaultMessage = "Specified cast is not valid.";
        public InvalidCastException() : base(DefaultMessage)
        { 
        }

        public InvalidCastException(string message) : base(message ?? DefaultMessage)
        {
        }
    }
}
