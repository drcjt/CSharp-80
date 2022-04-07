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
            const int MAX_LENGTH = 80;

            var snakeXs = stackalloc int[MAX_LENGTH];
            var snakeYs = stackalloc int[MAX_LENGTH];

            // Initialise Xs & Ys to -1
            for (int i = 0; i < MAX_LENGTH; i++)
            {
                snakeXs[i] = -1;
                snakeYs[i] = -1;
            }

            const int bits = 128 * 48;
            const int boardSize = (bits) / 32;
            var board = stackalloc int[boardSize];

            for (int i = 0; i < 192; i++)
            {
                board[i] = 0;
            }

            var startX = GraphicHelper.WrapAround((byte)_random.Next(), Graphics.ScreenWidth / 2);
            var startY = GraphicHelper.WrapAround((byte)_random.Next(), Graphics.ScreenHeight);

            var s = new Snake(startX, startY, snakeXs, snakeYs, board, MAX_LENGTH, (Direction)(_random.Next() % 4));

            MakeFood(s, out int foodX, out int foodY);
            SetPixel(foodX, foodY, Color.White);
            SetPixel(s.HeadX, s.HeadY, Color.White);

            SetBoardItem(board, s.HeadX, s.HeadY, true);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo ki = Console.ReadKey(true);
                    int kc = ki.KeyChar;
                    switch (kc)
                    {
                        case 10: s.SetCourse(Direction.Down); break;
                        case 91: s.SetCourse(Direction.Up); break;
                        case 8: s.SetCourse(Direction.Left); break;
                        case 9: s.SetCourse(Direction.Right); break;
                    }
                }

                var oldTailX = s.TailX;
                var oldTailY = s.TailY;

                if (!s.Update())
                {
                    return Result.Loss;
                }

                SetPixel(oldTailX, oldTailY, Color.Black);
                SetPixel(s.HeadX, s.HeadY, Color.White);

                SetBoardItem(board, oldTailX, oldTailY, false);
                SetBoardItem(board, s.HeadX, s.HeadY, true);

                if (s.HitTest(foodX, foodY))
                {
                    if (s.Extend())
                    {
                        MakeFood(s, out foodX, out foodY);
                        SetPixel(foodX, foodY, Color.White);
                    }
                    else
                    {
                        return Result.Win;
                    }
                }

                Thread.Sleep(30);
            }
        }

        private static void SetPixel(int x, int y, Color color)
        {
            Graphics.SetPixel(x * 2, y, color);
            Graphics.SetPixel((x * 2) + 1, y, color);
        }

        void MakeFood(in Snake snake, out int foodX, out int foodY)
        {
            do
            {
                foodX = GraphicHelper.WrapAround((byte)_random.Next(), Graphics.ScreenWidth / 2);
                foodY = GraphicHelper.WrapAround((byte)_random.Next(), Graphics.ScreenHeight);
            }
            while (snake.SnakeHit(foodX, foodY));
        }

        private static unsafe void SetBoardItem(int* board, int x, int y, bool value)
        {
            int bitIndex = (y * 128) + x;
            int bitMask = 1 << bitIndex;

            if (value)
            {
                board[bitIndex >> 5] |= bitMask;
            }
            else
            {
                board[bitIndex >> 5] &= ~bitMask;
            }
        }

        public unsafe static void Main()
        {
            Console.Clear();

            //FrameBuffer fb = new FrameBuffer();
            while (true)
            {
                Game g = new Game((uint)Environment.TickCount);
                Result result = g.Run();

                if (result == Result.Win)
                {
                    Console.WriteLine("You win");

                    Thread.Sleep(5000);

                    // TODO: This seems to return immediately???
                    //Console.ReadKey(intercept: true);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("You lose");

                    break;
                }

                //fb.Render();
            }
        }
    }
}
