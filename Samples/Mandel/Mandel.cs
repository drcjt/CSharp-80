using dnlib.DotNet.Resources;
using System;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Mandelbrot
{
    public static class Program
    {
        public static void Main()
        {
            int leftEdge = -300;
            int rightEdge = 300;
            int topEdge = 300;
            int bottomEdge = -300;

            int xStep = (rightEdge - leftEdge) / (Console.WindowWidth - 1);
            leftEdge = -((xStep * (Console.WindowWidth - 1)) / 2);
            rightEdge = -leftEdge;
            int yStep = 15;

            int maxIterations = 200;

            for (int y0 = topEdge; y0 > bottomEdge; y0 -= yStep)
            {
                for (int x0 = leftEdge; x0 < rightEdge; x0 += xStep)
                {
                    int x = 0;
                    int y = 0;
                    int i = 0;
                    char c = ' ';

                    while (i < maxIterations)
                    {
                        int x_x = (x * x) / 200;
                        int y_y = (y * y) / 200;
                        if (x_x + y_y > 800)
                        {
                            c = (char)('0' + i);
                            if (i > 9)
                            {
                                c = '@';
                            }
                            i = maxIterations;
                        }
                        y = x * y / 100 + y0;
                        x = x_x - y_y + x0;
                        i++;
                    }
                    Console.Write(c);
                }

                Console.WriteLine();
            }
        }
    }
}