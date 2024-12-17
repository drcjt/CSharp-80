namespace System
{
    public class Exception
    {
        internal string? _message = null;

        public Exception() { }

        public Exception(string message)
        {
            _message = message;
        }

        public string Message => _message ?? "";
    }
}
