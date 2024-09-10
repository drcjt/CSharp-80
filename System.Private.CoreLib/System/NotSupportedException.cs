namespace System
{
    public class NotSupportedException : Exception
    {
        public NotSupportedException() : base("Specified method is not supported.")
        { }
    }
}
