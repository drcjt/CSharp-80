using System;
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

        private void Run(/* ref FrameBuffer fb */)
        {
            int gameTime = Environment.TickCount;
            while (true)
            {
                gameTime += 100;

                int delay = gameTime - Environment.TickCount;
                if (delay > 0)
                    Thread.Sleep(delay);
                else
                    gameTime = Environment.TickCount;

                Console.WriteLine(_random.Next());
            }
        }

        public static void Main()
        {
            //FrameBuffer fb = new FrameBuffer();
            while (true)
            {                
                Game g = new Game((uint)Environment.TickCount);
                g.Run();
                //bool result = g.Run(ref fb);

                //fb.Render();
            }
        }
    }
}
