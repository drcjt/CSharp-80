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

            Graphics.SetPixel(s.headX, s.headY, Color.White);

            int gameTime = Environment.TickCount;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo ki = Console.ReadKey(true);
                    int kc = ki.KeyChar;
                    switch (kc)
                    {
                        case 10: dx = 0; dy = 1; break;
                        case 91: dx = 0; dy = -1; break;
                        case 8: dx = -1; dy = 0; break;
                        case 9: dx = 1; dy = 0; break;
                    }
                }

                // TODO: This timing mechanism doesn't quite work as get occasional
                // pauses which I believe are due to TickCount wrapping around
                int delay = Environment.TickCount - gameTime;
                if (delay > 25)
                {
                    gameTime = Environment.TickCount;

                    s.headX = WrapAround(s.headX + dx, Graphics.ScreenWidth);
                    s.headY = WrapAround(s.headY + dy, Graphics.ScreenHeight);

                    Graphics.SetPixel(s.headX, s.headY, Color.White);
                }
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
