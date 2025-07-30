using System;
using System.Drawing;

namespace Balls
{
    public class Ball(int x, int y, int velocityX, int velocityY)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int VelocityX { get; set; } = velocityX;
        public int VelocityY { get; set; } = velocityY;

        public void Move(Ball[] balls)
        {
            X += VelocityX;
            Y += VelocityY;

            // Check for collision with walls
            if (X <= 0 || X >= Graphics.ScreenWidth)
            {
                VelocityX = -VelocityX; // Reverse horizontal direction
            }
            if (Y <= 0 || Y >= Graphics.ScreenHeight)
            {
                VelocityY = -VelocityY; // Reverse vertical direction
            }

            X = Math.Clamp(X, 0, Graphics.ScreenWidth);
            Y = Math.Clamp(Y, 0, Graphics.ScreenHeight);
        }
    }

    public static class Program
    {
        public static void Main()
        {
            int numberOfBalls = GetNumberOfBalls();

            Ball[] balls = CreateBalls(numberOfBalls);

            foreach (Ball ball in balls)
            {
                Graphics.SetPixel(ball.X, ball.Y, Color.White);
            }

            bool bouncing = true;
            while (bouncing)
            {
                foreach (Ball ball in balls)
                {
                    Graphics.SetPixel(ball.X, ball.Y, Color.Black);

                    ball.Move(balls);

                    Graphics.SetPixel(ball.X, ball.Y, Color.White);

                    // Check for user input to exit
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Q)
                        {
                            bouncing = false;
                            break;
                        }
                    }
                }

                CheckForCollisions(balls);
            }
        }

        private static int GetNumberOfBalls()
        {
            Console.WriteLine("Press 'Q' to quit.");

            int numberOfBalls;
            do
            {
                Console.WriteLine("Enter number of Balls 1 to 20?");
                numberOfBalls = int.Parse(Console.ReadLine());
            } while (numberOfBalls < 1 || numberOfBalls > 20);

            Console.Clear();
            return numberOfBalls;
        }

        private static void CheckForCollisions(Ball[] balls)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                // Check for collision with other balls
                for (int j = i + 1; j < balls.Length; j++)
                {
                    if (balls[i].X == balls[j].X && balls[i].Y == balls[j].Y)
                    {
                        // Reverse direction upon collision
                        balls[i].VelocityX = -balls[i].VelocityX;
                        balls[i].VelocityY = -balls[i].VelocityY;

                        // Move the ball slightly away to prevent sticking
                        balls[i].X += balls[i].VelocityX;
                        balls[i].Y += balls[i].VelocityY;

                        // Reverse the other ball's direction as well
                        balls[j].VelocityX = -balls[j].VelocityX;
                        balls[j].VelocityY = -balls[j].VelocityY;

                        Console.Beep();
                    }
                }
            }
        }

        private static Ball[] CreateBalls(int numberOfBalls)
        {
            Ball[] balls = new Ball[numberOfBalls];

            var random = new Random();

            for (int i = 0; i < balls.Length; i++)
            {
                // Randomly place balls within the console window
                int x = random.Next(0, Graphics.ScreenWidth);
                int y = random.Next(0, Graphics.ScreenHeight);

                int vx = 0, vy = 0;

                while (vx == 0 || vy == 0)
                {
                    // Randomly assign velocities, ensuring at least one is non-zero
                    vx = random.Next(-2, 3); // Velocity can be -1, 0, or 1
                    vy = random.Next(-2, 3);
                }

                balls[i] = new Ball(x, y, vx, vy);
            }

            return balls;
        }
    }
}
