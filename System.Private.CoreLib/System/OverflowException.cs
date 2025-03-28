namespace System
{
    public class OverflowException : ArithmeticException
    {
        public OverflowException() { }
        public OverflowException(string message) : base(message) { }
    }
}
