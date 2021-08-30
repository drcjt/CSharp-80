using System;

namespace MiniBCL
{
    public static class Program
    {
        public static int Main()
        {
            Console.Clear();

            short a = 123;
            Console.WriteLine(-a);

            Console.WriteLine(5 + 4);       // output: 9

            ushort x = 45123;
            ushort y = (ushort)(x + 1);
            Console.WriteLine(y);

            //PostIncrementOperator();

            return 42;
        }

        // TODO: This requires dup to be implemented
        private static void PostIncrementOperator()
        {
            int i = 3;
            Console.WriteLine(i);   // output: 3
            Console.WriteLine(i++); // output: 3
            Console.WriteLine(i);   // output: 4
        }
    }
}
