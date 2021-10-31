﻿using System;
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

            Snake s = new Snake(WrapAround((byte)_random.Next(), Graphics.ScreenWidth / 2), WrapAround((byte)_random.Next(), Graphics.ScreenHeight), snakeXs, snakeYs, MAX_LENGTH, (Direction)(_random.Next() % 4));

            MakeFood(s, out int foodX, out int foodY);
            SetPixel(foodX, foodY, Color.White);

            SetPixel(s.HeadX, s.HeadY, Color.White);

            int gameTime = Environment.TickCount;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo ki = Console.ReadKey(true);
                    int kc = ki.KeyChar;
                    switch (kc)
                    {
                        case 10: s.Course = Direction.Down; break;
                        case 91: s.Course = Direction.Up; break;
                        case 8: s.Course = Direction.Left; break;
                        case 9: s.Course = Direction.Right; break;
                    }
                }


                Thread.Sleep(50);

                SetPixel(s.TailX, s.TailY, Color.Black);

                if (!s.Update())
                {
                    return Result.Loss;
                }

                SetPixel(s.HeadX, s.HeadY, Color.White);

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
                foodX = WrapAround((byte)_random.Next(), Graphics.ScreenWidth / 2);
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

                    Thread.Sleep(5000);

                    // TODO: This seems to return immediately???
                    //Console.ReadKey(intercept: true);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("You lose");

                    Thread.Sleep(5000);

                    Console.Clear();
                }

                //fb.Render();
            }
        }
    }
}
