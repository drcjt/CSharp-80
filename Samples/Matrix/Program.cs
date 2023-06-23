using System;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Matrix
{
    public static class Program
    {
        static int Height;
        const int Width = 20;
        public unsafe static void Main()
        {
            Height = Console.WindowHeight - 1;

            var random = new Random(); // NOSONAR

            // TODO: Cannot use width here as this generates IL using
            // the mul.ovf.un opcode which is not supported yet ..
            int* y = stackalloc int[20];

            for (int x = 0; x < Width; x++)
            {
                y[x] = random.Next(Height);
            }

            Console.Clear();

            for (int run = 0; run < 100; run++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var t = y[x];
                    Console.SetCursorPosition(x, t);
                    Console.Write('A');

                    Console.SetCursorPosition(x, InScreenYPosition(t - 6));
                    Console.Write(' ');

                    y[x] = InScreenYPosition(t + 1);
                }
            }
        }

        public static int InScreenYPosition(int yPosition)
        {
            if (yPosition >= Height)
                return 0;
            else if (yPosition < 0)
                return yPosition + Height;
            else
                return yPosition;
        }
    }
}
