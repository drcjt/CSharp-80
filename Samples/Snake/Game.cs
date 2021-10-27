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

        private unsafe Result Run(/* ref FrameBuffer fb */)
        {
            // TOOD: this is the max length right now as IX/IY offset
            // addressing limits the stack size.
            const int MAX_LENGTH = 20;

            var snakeXs = stackalloc int[MAX_LENGTH];
            var snakeYs = stackalloc int[MAX_LENGTH];

            Snake s = new Snake(WrapAround((byte)_random.Next(), Graphics.ScreenWidth), WrapAround((byte)_random.Next(), Graphics.ScreenHeight), snakeXs, snakeYs, MAX_LENGTH);

            MakeFood(s, out int foodX, out int foodY);
            Graphics.SetPixel(foodX, foodY, Color.White);

            int dx = 1;
            int dy = 0;

            Graphics.SetPixel(s.HeadX, s.HeadY, Color.White);

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
                if (delay > 50)
                {
                    gameTime = Environment.TickCount;

                    s.Next(WrapAround(s.HeadX + dx, Graphics.ScreenWidth), WrapAround(s.HeadY + dy, Graphics.ScreenHeight));

                    if (s.HitTest(foodX, foodY))
                    {
                        if (s.Extend())
                        {
                            MakeFood(s, out foodX, out foodY);
                            Graphics.SetPixel(foodX, foodY, Color.White);
                        }
                        else
                        {
                            return Result.Win;
                        }
                    }

                    Graphics.SetPixel(s.HeadX, s.HeadY, Color.White);
                    Graphics.SetPixel(s.TailX, s.TailY, Color.Black);
                }
            }
        }

        void MakeFood(in Snake snake, out int foodX, out int foodY)
        {
            do
            {
                foodX = WrapAround((byte)_random.Next(), Graphics.ScreenWidth);
                foodY = WrapAround((byte)_random.Next(), Graphics.ScreenHeight);
            }
            while (snake.HitTest(foodX, foodY));
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

                if (result == Result.Win)
                {
                    Console.WriteLine("You win");

                    // TODO: This seems to return immediately???
                    Console.ReadKey(intercept: true);
                    Console.Clear();
                }

                //fb.Render();
            }
        }
    }
}
