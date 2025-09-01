namespace System
{
    public class DivideByZeroException : ArithmeticException
    {
        public DivideByZeroException() { }
        public DivideByZeroException(string message) : base(message) { }
    }
}
