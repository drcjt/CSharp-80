namespace System.Runtime.CompilerServices
{
    public static class RuntimeHelpers
    {
        public static unsafe int OffsetToStringData => 0; //sizeof(IntPtr) + sizeof(int);

        // TODO: This should be removed when better support for reflection has been
        // added via Type class.
        public unsafe static bool HasCctor<T>() => EETypePtr.EETypePtrOf<T>().HasCctor;
    }
}