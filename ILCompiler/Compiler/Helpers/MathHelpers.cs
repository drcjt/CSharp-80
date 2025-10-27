namespace ILCompiler.Compiler.Helpers
{
    internal static class MathHelpers
    {
        public static int Gcd(int a, int b)
        {
            while (a != 0)
            {
                (a, b) = (b % a, a);
            }

            return b;
        }

        public static bool IsPow2(int i) => (i > 0 && ((i - 1) & i) == 0);

        public static int GetLog2(int i)
        {
            int r = 0;
            while ((i >>= 1) != 0)
            {
                ++r;
            }
            return r;
        }
    }
}
