using System.Runtime;

namespace System
{
    public partial class String
    {
        public unsafe static string Concat(string str0, string str1)
        {
            string result = RuntimeImports.NewString(str0.Length + str1.Length);

            FillStringChecked(result, 0, str0);
            FillStringChecked(result, str0.Length, str1);

            return result;
        }

        private unsafe static void FillStringChecked(string dest, int destPos, string src)
        {
            // TODO: Reimplement using unsafe intrinsics
            fixed (char* destPtr = &(dest._firstChar))
            {
                var destination = destPtr + destPos;

                fixed (char* srcPtr = &(src._firstChar))
                {
                    Buffer.Memmove((byte*)destination, (byte*)srcPtr, src.Length * 2);
                }
            }
        }
    }
}
