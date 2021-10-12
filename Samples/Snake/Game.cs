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

                ConsoleKeyInfo ki = Console.ReadKey(true);
                if (ki.KeyChar == 10)
                {
                    dx = 0; dy = 1;
                }
                else if (ki.KeyChar == 91)
                {
                    dx = 0; dy = -1;
                }
                else if (ki.KeyChar == 8)
                {
                    dx = -1; dy = 0;
                }
                else if (ki.KeyChar == 9)
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
