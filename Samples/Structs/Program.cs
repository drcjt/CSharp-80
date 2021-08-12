using System;

namespace Structs
{
    public struct SimpleVector
    {
        public int X;
        public int Y;

        public SimpleVector(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            SimpleVector vector = new SimpleVector(3, 4);

            Console.Write(vector.X);
            Console.Write(vector.Y);
        }
    }
}
