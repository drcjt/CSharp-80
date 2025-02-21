using System.Runtime.InteropServices;

namespace DllImportAttributeTests
{
    public static class Test
    {
        [DllImport("Native.asm", EntryPoint = "GetZero")]
        private static extern int GetZero();

        [DllImport("Native.asm", EntryPoint = "GetValue")]
        private static extern int GetValue(int value);

        public static int Main()
        {
            if (GetZero() != 0)
            {
                return 100;
            }
            if (GetValue(25) != 25)
            {
                return 200;
            }
            return 0;
        }
    }
}