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
            Snake s = new Snake((byte)(_random.Next() % 128), (byte)(_random.Next() % 48));

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
                if (s.headX > 127)
                {
                    s.headX = 0;
                }
                if (s.headY > 47)
                {
                    s.headY = 0;
                }

                if (s.headX < 0)
                {
                    s.headX = 127;
                }
                if (s.headY < 0)
                {
                    s.headY = 47;
                }
            }
        }

        public static void StarBurst()
        {
            Console.Clear();

            Random random = new Random((uint)Environment.TickCount);

            for (int i = 0; i < 30; i++)
            {
                int x = (byte)random.Next() % 127;
                int y = (byte)random.Next() % 47;

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
                    Graphics.SetPixel((byte)random.Next() % 127, (byte)random.Next() % 47, Color.White);
                }

                for (int i = 0; i < 1000; i++)
                {
                    Graphics.SetPixel((byte)random.Next() % 127, (byte)random.Next() % 47, Color.Black);
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
