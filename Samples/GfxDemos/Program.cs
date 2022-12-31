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

            Random random = new Random((uint)Environment.TickCount);

            for (int i = 0; i < 10; i++)
            {
                var x = ((byte)random.Next()) % 128;
                var y = ((byte)random.Next()) % 48;

                var width = ((byte)random.Next()) % (128 - x);
                var height = ((byte)random.Next()) % (48 - y);

                Graphics.DrawEllipse(x, y, width, height);
            }
        }

        public static void StarField()
        {
            Console.Clear();

            Random random = new Random((uint)Environment.TickCount);

            int loops = 5;
            while (loops > 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Graphics.SetPixel((byte)random.Next() % Graphics.ScreenWidth, (byte)random.Next() % Graphics.ScreenHeight, Color.White);
                }

                for (int i = 0; i < 250; i++)
                {
                    Graphics.SetPixel((byte)random.Next() % Graphics.ScreenWidth, (byte)random.Next() % Graphics.ScreenHeight, Color.Black);
                }

                loops--;
            }
        }

        public static void StarBurst()
        {
            Console.Clear();

            Random random = new Random((uint)Environment.TickCount);

            for (int i = 0; i < 30; i++)
            {
                int x = (byte)random.Next() % Graphics.ScreenWidth;
                int y = (byte)random.Next() % Graphics.ScreenHeight;

                Graphics.DrawLine(0, 0, x, y);
            }
        }
    }
}
