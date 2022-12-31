using System;
using System.Drawing;

namespace GfxDemos
{
    public static class Program
    {
        public static void Main()
        {
            while (true)
            {
                RandomCircles();
                StarBurst();
                StarField();
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

            int loops = 5;
            while (loops > 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Graphics.SetPixel(random.Next(Graphics.ScreenWidth), random.Next(Graphics.ScreenHeight), Color.White);
                }

                for (int i = 0; i < 250; i++)
                {
                    Graphics.SetPixel(random.Next(Graphics.ScreenWidth), random.Next(Graphics.ScreenHeight), Color.Black);
                }

                loops--;
            }
        }

        public static void StarBurst()
        {
            Console.Clear();

            Random random = new Random();

            for (int i = 0; i < 30; i++)
            {
                int x = random.Next(Graphics.ScreenWidth);
                int y = random.Next(Graphics.ScreenHeight);

                Graphics.DrawLine(Pens.White, 0, 0, x, y);
            }
        }
    }
}
