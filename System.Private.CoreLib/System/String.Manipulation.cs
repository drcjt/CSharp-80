using Internal.Runtime;
using Internal.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime;

namespace System
{
    public partial class String
    {
        public unsafe static string Concat(string str0, string str1)
        {
            string result = RuntimeImports.NewString(EEType.Of<string>(), str0.Length + str1.Length);

            FillStringChecked(result, 0, str0);
            FillStringChecked(result, str0.Length, str1);

            return result;
        }

        private unsafe static void FillStringChecked(string dest, int destPos, string src)
        {     
            Buffer.Memmove(
                ref Unsafe.Add(ref dest._firstChar, destPos),
                ref src._firstChar,
                (uint)src.Length);
        }

        public string Substring(int startIndex, int length) 
        { 
            if (length == 0)
            {
                return Empty;
            }

            if (length == Length)
            {
                Debug.Assert(startIndex == 0);
                return this;
            }

            if (startIndex > Length || length > (Length - startIndex))
            {
                // Should throw exception here but return null for now
                return null;
            }

            return InternalSubstring(startIndex, length);
        }

        private unsafe string InternalSubstring(int startIndex, int length)
        {
            string result = RuntimeImports.NewString(EEType.Of<string>(), length);

            Buffer.Memmove(ref result._firstChar, ref Unsafe.Add(ref _firstChar, startIndex), (uint)length);

            return result;
        }
    }
}
