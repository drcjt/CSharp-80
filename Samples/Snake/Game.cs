using System;
using System.Drawing;
using System.Threading;

namespace Snake
{
    struct Game
    {
        enum Result
        {
            Win, Loss
        }

        private Random _random;

        private Game(uint randomSeed)
        {
            _random = new Random(randomSeed);
        }

        private Result Run(/* ref FrameBuffer fb */)
        {
            Snake s = new Snake(WrapAround((byte)_random.Next(), Graphics.ScreenWidth), WrapAround((byte)_random.Next(), Graphics.ScreenHeight));

            int dx = 1;
            int dy = 0;

            int gameTime = Environment.TickCount;
            while (true)
            {
                Graphics.SetPixel(s.headX, s.headY, Color.White);

                gameTime += 100;

                int delay = gameTime - Environment.TickCount;
                if (delay > 0)
                    Thread.Sleep(delay);
                else
                    gameTime = Environment.TickCount;

                //Graphics.SetPixel(s.headX, s.headY, Color.Black);

                int keyChar = Console.KbdScan();
                if (keyChar == 10)
                {
                    dx = 0; dy = 1;
                }
                else if (keyChar == 91)
                {
                    dx = 0; dy = -1;
                }
                else if (keyChar == 8)
                {
                    dx = -1; dy = 0;
                }
                else if (keyChar == 9)
                {
                    dx = 1; dy = 0;
                }

                s.headX += dx;
                s.headY += dy;

                // Wrap around
                s.headX = WrapAround(s.headX, Graphics.ScreenWidth);
                s.headY = WrapAround(s.headY, Graphics.ScreenHeight);
            }
        }

        private static int WrapAround(int coordinate, int max)
        {
            coordinate %= max + 1;
            return (coordinate < 0) ? max : coordinate;
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

        public static void StarField()
        {
            Console.Clear();

            Random random = new Random((uint)Environment.TickCount);

            while (true)
            {
                for (int i = 0; i < 100; i++)
                {
                    Graphics.SetPixel((byte)random.Next() % Graphics.ScreenWidth, (byte)random.Next() % Graphics.ScreenHeight, Color.White);
                }

                for (int i = 0; i < 1000; i++)
                {
                    Graphics.SetPixel((byte)random.Next() % Graphics.ScreenWidth, (byte)random.Next() % Graphics.ScreenHeight, Color.Black);
                }
            }
        }

        public static void Main()
        {
            Console.Clear();

            //FrameBuffer fb = new FrameBuffer();
            while (true)
            {                
                Game g = new Game((uint)Environment.TickCount);
                Result result = g.Run(/* ref fb */);

                //fb.Render();
            }
        }
    }
}
