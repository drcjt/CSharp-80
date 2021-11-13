namespace System.Runtime.CompilerServices
{
    public class RuntimeHelpers
    {
        public static unsafe int OffsetToStringData => 0; //sizeof(IntPtr) + sizeof(int);
    }
}
