namespace System.Runtime.CompilerServices
{
    public static class RuntimeHelpers
    {
        public static unsafe int OffsetToStringData => 0; //sizeof(IntPtr) + sizeof(int);

        public unsafe static bool HasCctor<T>() => EETypePtr.EETypePtrOf<T>()->GetFlags() == 1;

    }
}