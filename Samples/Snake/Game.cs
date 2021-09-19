using System;

namespace Snake
{
    struct Game
    {
        /*
        enum Result
        {
            Win, Loss
        }
        */

        private Random _random;

        private Game(uint randomSeed)
        {
            _random = new Random(randomSeed);
        }

        private bool Run(ref FrameBuffer fb)
        {
            return false;
        }

        public static void Main()
        {
            //FrameBuffer fb = new FrameBuffer();

            int loopCount = 4;
            while (loopCount > 0)
            {
                Game g = new Game(123);
                
                // Just for testing
                Console.WriteLine(g._random.Next());

                //bool result = g.Run(ref fb);

                //fb.Render();
                loopCount -= 1;
            }
        }
    }
}
