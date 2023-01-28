using ILCompiler.Compiler.Importer;
using System;
using System.Drawing;

namespace GfxDemos
{
    public static class Program
    {
        public static void Main()
        {
            Console.Clear();

            var black = Pens.Black;
            Console.WriteLine((int)black.Color);
            Console.WriteLine((int)Color.Black);

            Graphics.DrawLine(Pens.White, 0, 10, 127, 10);
            Graphics.DrawLine(Pens.Black, 0, 10, 127, 10);

            Graphics.SetPixel(0, 10, Color.Black);
            /*

            while (true)
            {
                RandomCircles();
                StarBurst();
                StarField();
                Spiral(Pens.White);
                FillScreen(Pens.Black);
            }
            */
        }

        public static void FillScreen(Pen pen)
        {
            for (int y = 0; y < Graphics.ScreenHeight; y++)
            {
                Graphics.DrawLine(pen, 0, y, 127, y);
            }
        }

        public static void Spiral(Pen pen, int startx = 0, int endx = 127, int starty = 0, int endy = 47)
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

            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                var x = random.Next(128);
                var y = random.Next(48);

                var width = random.Next(128) % (128 - x);
                var height = random.Next(48) % (48 - y);

                Graphics.DrawEllipse(Pens.White, x, y, width, height);
            }
        }

        public static void StarField()
        {
            Console.Clear();

            Random random = new Random();

            for (int i = 0; i < 50; i++)
            {
                Graphics.SetPixel(random.Next(Graphics.ScreenWidth), random.Next(Graphics.ScreenHeight), Color.White);
            }
        }

        public static void StarBurst()
        {
            Console.Clear();

            Random random = new Random();

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
