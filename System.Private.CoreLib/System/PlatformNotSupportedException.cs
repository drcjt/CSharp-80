namespace System
{
    public class PlatformNotSupportedException : Exception
    {
        public PlatformNotSupportedException() : base("Operation is not supported on this platform.")
        { }
    }
}
