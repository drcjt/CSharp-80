using Internal.Runtime;

namespace System.Runtime.CompilerServices
{
    public static class RuntimeHelpers
    {
        // Number of bytes from the address pointed to by a reference to a String
        // to the first 16-bit character in the String. Skip over the EEType pointer
        // and String length.
        // This property allows C#'s fixed statement to work on Strings.
        public static unsafe int OffsetToStringData => 4;

        // TODO: This should be removed when better support for reflection has been
        // added via Type class.
        public unsafe static bool HasCctor<T>() => EEType.Of<T>()->HasCctor;
    }
}