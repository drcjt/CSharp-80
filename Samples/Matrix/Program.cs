using System;

namespace Matrix
{
    public static class Program
    {
        const int Height = 16;
        const int Width = 20;
        public unsafe static void Main()
        {
            var random = new Random((uint)Environment.TickCount);


            // TODO: Cannot use width here as this generates IL using
            // the mul.ovf.un opcode which is not supported yet ..
            int* y = stackalloc int[20];

            for (int x = 0; x < Width; x++)
            {
                y[x] = (int)random.Next() % Height;
            }

            Console.Clear();

            while (true)
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
            if (yPosition >= 0 && yPosition < Height)
                return yPosition;
            else if (yPosition < 0)
                return yPosition + Height;
            else
                return 0;
        }
    }
}
