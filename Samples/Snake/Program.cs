using System;

namespace MiniBCL
{
    public static class Program
    {
        public static int Main()
		{
            Console.Clear();

            uint val = Next(0);
            Console.WriteLine(val);
            val = Next(val);
            Console.WriteLine(val);
            val = Next(val);
            Console.WriteLine(val);
            val = Next(val);
            Console.WriteLine(val);
            val = Next(val);
            Console.WriteLine(val);

            return 0;
        }

        public static uint Next(uint val)
        {
            uint next = (1103515245 * val + 12345) % 2147483648;
            return next;
        }
    }
}
