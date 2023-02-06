﻿namespace System
{
    public sealed partial class String
    {
        // The layout of the string type is a contract with the compiler

        // Note the real dotnet runtime uses an int for the string length
        // but on a Z80 we can only address up to 64k so here we use a short
        public readonly short _length;
        public char _firstChar;

        public int Length => (int)_length;

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
