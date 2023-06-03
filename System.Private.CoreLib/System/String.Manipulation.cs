using Internal.Runtime.CompilerServices;
using System.Runtime;

namespace System
{
    public partial class String
    {
        public unsafe static string Concat(string str0, string str1)
        {
            string result = RuntimeImports.NewString(EETypePtr.EETypePtrOf<String>(), str0.Length + str1.Length);

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
    }
}
