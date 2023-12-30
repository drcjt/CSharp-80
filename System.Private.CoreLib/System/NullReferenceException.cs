namespace System
{
    public sealed class NullReferenceException : Exception
    {
        public NullReferenceException() : base("Object reference not set to an instance of an object.")
        { }
    }
}
