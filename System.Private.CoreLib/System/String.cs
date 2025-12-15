using Internal.Runtime;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DynamicDependency("Ctor(System.Char[])")]
        public extern String(char[] value);

        internal unsafe static string Ctor(char[] value)
        {
            if (value == null || value.Length == 0)
            {
                return Empty;
            }

            string result = RuntimeImports.NewString(EEType.Of<string>(), (nuint)value.Length);
            Buffer.Memmove(ref result._firstChar, ref value[0], (uint)result.Length);
            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DynamicDependency("Ctor(System.ReadOnlySpan`1<System.Char>)")]
        public extern String(ReadOnlySpan<char> value);

        internal unsafe static string Ctor(ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
            {
                return Empty;
            }

            string result = RuntimeImports.NewString(EEType.Of<string>(), (nuint)value.Length);
            Buffer.Memmove(ref result._firstChar, ref MemoryMarshal.GetReference(value), (uint)result.Length);
            return result;
        }

        internal unsafe static string CreateFromChar(char c)
        {
            string result = RuntimeImports.NewString(EEType.Of<string>(), 1);
            result._firstChar = c;
            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DynamicDependency("Ctor(System.Char,System.Int32)")]
        public extern String(char c, int count);

        internal unsafe static string Ctor(char c, int count)
        {
            string result = RuntimeImports.NewString(EEType.Of<string>(), (nuint)count);
            SpanHelpers.Fill(ref result._firstChar, (uint)count, c);
            return result;
        }


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

        internal ref char GetRawStringData() => ref _firstChar;

        public static implicit operator ReadOnlySpan<char>(string? value)
        {
            return value != null ? new ReadOnlySpan<char>(ref value.GetRawStringData(), value.Length) : default;
        }
    }
}
