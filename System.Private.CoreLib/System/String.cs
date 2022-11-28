using System.Runtime.CompilerServices;

namespace System
{
    public sealed class String
    {
        public readonly int Length;
        public char _firstChar;

        [System.Runtime.CompilerServices.IndexerName("Chars")]
        public unsafe char this[int index]
        {
            [System.Runtime.CompilerServices.Intrinsic]
            get
            {
                return Internal.Runtime.CompilerServices.Unsafe.Add(ref _firstChar, index);
            }
        }
    }
}
