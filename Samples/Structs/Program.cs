using System;

namespace Structs
{
    public struct Nested
    {
        public int Length;
        public int Width;

        public Nested(int length, int width)
        {
            Length = length;
            Width = width;
        }
    }

    public struct SimpleVector
    {
        public int X;
        public int Y;
        public Nested N;

        public SimpleVector(int x, int y, Nested n)
        {
            X = x;
            Y = y;
            N = n;
        }
    }

    public static class Program
    {
        public static void Main()
        {
            Nested n = new Nested(25, 7);
            Nested n2 = new Nested(49, 8);
            SimpleVector vector = new SimpleVector(3, 4, n);

            vector.N = n2;

            var length = Test(vector);
            Console.WriteLine(length);

        }
        public static int Test(SimpleVector v)
        {
            var n = v.N;
            return n.Length;
        }
    }
}
