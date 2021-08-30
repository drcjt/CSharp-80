using System;

namespace Structs
{
    public struct Nested
    {
        public int Length;

        public Nested(int length)
        {
            Length = length;
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
            Nested n = new Nested(25);
            SimpleVector vector = new SimpleVector(3, 4, n);

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
