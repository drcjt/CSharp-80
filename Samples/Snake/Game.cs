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
            TestRandom();
            /*
            FrameBuffer fb = new FrameBuffer();

            while (true)
            {
                Game g = new Game(123);

                bool result = g.Run(ref fb);

                fb.Render();
            }
            */
        }

        private static void TestRandom()
        {
            Console.Clear();

            // TODO: This doesn't do anything at the minute as initobj is unimplemented
            Random random = new Random();

            random._val = 0;
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
        }
    }
}
