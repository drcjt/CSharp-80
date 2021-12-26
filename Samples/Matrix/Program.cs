using System;

namespace Matrix
{
    public static class Program
    {
        public unsafe static void Main()
        {
            var random = new Random((uint)Environment.TickCount);

            int height = 16;
            int width = 20;

            int* y = stackalloc int[64];    // TODO: If use width here then get problems!!

            for (int x = 0; x < width; x++)
            {
                y[x] = (int)random.Next() % height;
            }

            Console.Clear();

            while (true)
            {
                for (int x = 0; x < width; x++)
                {
                    var t = y[x];
                    Console.SetCursorPosition(x, t);
                    Console.Write('A');

                    Console.SetCursorPosition(x, inScreenYPosition(t - 6, height));
                    Console.Write(' ');

                    y[x] = inScreenYPosition(t + 1, height);
                }
            }
        }

        public static int inScreenYPosition(int yPosition, int height)
        {
            if (yPosition < 0)
                return yPosition + height;
            else if (yPosition < height)
                return yPosition;
            else
                return 0;
        }
    }
}
