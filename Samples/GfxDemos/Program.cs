﻿using System;
using System.Drawing;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace GfxDemos
{
    public static class Program
    {
        public static void Main()
        {            
            Console.Clear();

            for (int run = 0; run < 10; run++)
            {
                RandomCircles();
                StarBurst();
                StarField();
                Spiral(Pens.White);
                FillScreen(Pens.Black);
            }
        }

        public static void FillScreen(Pen pen)
        {
            for (int y = 0; y < Graphics.ScreenHeight; y++)
            {
                Graphics.DrawLine(pen, 0, y, Graphics.ScreenWidth, y);
            }
        }

        public static void Spiral(Pen pen, int startx = 0, int endx = Graphics.ScreenWidth, int starty = 0, int endy = Graphics.ScreenHeight)
        {
            Graphics.DrawLine(pen, startx, starty, endx, starty);
            Graphics.DrawLine(pen, endx, starty, endx, endy);
            Graphics.DrawLine(pen, endx, endy, startx, endy);
            Graphics.DrawLine(pen, startx, endy, startx, starty + 1);

            if (endx - startx > 1 && endy - starty > 1)
            {
                Spiral(pen, startx + 1, endx - 1, starty + 1, endy - 1);
            }
        }

        public static void RandomCircles()
        {
            Console.Clear();

            Random random = new Random(); // NOSONAR

            for (int i = 0; i < 10; i++)
            {
                var x = random.Next(Graphics.ScreenWidth);
                var y = random.Next(Graphics.ScreenHeight);

                var width = random.Next(Graphics.ScreenWidth) % (Graphics.ScreenWidth - x);
                var height = random.Next(Graphics.ScreenHeight) % (Graphics.ScreenHeight - y);

                Graphics.DrawEllipse(Pens.White, x, y, width, height);
            }
        }

        public static void StarField()
        {
            Console.Clear();

            Random random = new Random(); // NOSONAR

            for (int i = 0; i < 50; i++)
            {
                Graphics.SetPixel(random.Next(Graphics.ScreenWidth), random.Next(Graphics.ScreenHeight), Color.White);
            }
        }

        public static void StarBurst()
        {
            Console.Clear();

            Random random = new Random(); // NOSONAR

            int centerX = Graphics.ScreenWidth / 2;
            int centerY = Graphics.ScreenHeight / 2;

            for (int i = 0; i < 30; i++)
            {
                int x = random.Next(Graphics.ScreenWidth);
                int y = random.Next(Graphics.ScreenHeight);

                Graphics.DrawLine(Pens.White, centerX, centerY, x, y);
            }
        }
    }
}
