using Internal.Runtime;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace System
{
    public sealed partial class String : IEquatable<string>
    {
        // The layout of the string type is a contract with the compiler

        // Note the real dotnet runtime uses an int for the string length
        // but on a Z80 we can only address up to 64k so here we use a short
        public readonly short _length;
        public char _firstChar;

        public static readonly string Empty = "";

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

        internal unsafe static string Ctor(char[] value)
        {
            string result = RuntimeImports.NewString(EEType.Of<string>(), value.Length);
            Buffer.Memmove(ref result._firstChar, ref value[0], (uint)result.Length);
            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DynamicDependency("Ctor(System.Char[])")]
        public extern String(char[] value);

        public bool Contains(char value)
        {
            for (int i = 0; i < _length; i++)
            {
                if (this[i] == value)
                    return true;
            }

            return false;
        }

        public override string ToString() => this;
    }
}
