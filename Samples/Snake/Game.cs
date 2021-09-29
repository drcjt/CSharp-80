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

            int gameTime = Environment.TickCount;
            while (true)
            {
                Drawing.SetPixel(s.headX, s.headY, Color.White);

                gameTime += 100;

                int delay = gameTime - Environment.TickCount;
                if (delay > 0)
                    Thread.Sleep(delay);
                else
                    gameTime = Environment.TickCount;

                Drawing.SetPixel(s.headX, s.headY, Color.Black);

                // TODO: Changing this to s.headX++ causes issues need to investigate
                s.headX = s.headX + 1;
                if (s.headX > 127)
                {
                    s.headX = 0;
                    return Result.Loss;
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
