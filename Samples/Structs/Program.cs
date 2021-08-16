using System;

namespace Structs
{
    /*
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
    */

    public struct SimpleVector
    {
        public int X;
        public int Y;
        /* public Nested N; */

        public SimpleVector(int x, int y /* ,Nested n */)
        {
            X = x;
            Y = y;
            // N = n;
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            //Nested n = new Nested(true, 25);
            SimpleVector vector = new SimpleVector(3, 4 /*, n */);

            Console.Write(vector.X);
            Console.Write(vector.Y);
        }
    }
}
