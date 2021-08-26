using System;

namespace Structs
{
    public struct Nested
    {
        public bool Sucess;
        public short Length;

        public Nested(bool success, short length)
        {
            Sucess = success;
            Length = length;
        }
    }

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
        public static void Main()
        {
            Nested n = new Nested(true, 25);
            SimpleVector vector = new SimpleVector(3, 4);

            Console.WriteLine(vector.X);
            Console.WriteLine(vector.Y);
        }
    }
}
