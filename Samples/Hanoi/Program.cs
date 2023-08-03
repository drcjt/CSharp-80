using System;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Hanoi
{
    public static class Program
    {
        private static int NumberOfMoves;

        static void MoveTower(int disc, int from, int to, int use)
        {
            if (disc > 0)
            {
                NumberOfMoves++;
                MoveTower(disc - 1, from, use, to);
                Console.Write("Move disc ");
                Console.Write(disc);
                Console.Write(" from tower ");
                Console.Write(from);
                Console.Write(" to ");
                Console.WriteLine(to);
                MoveTower(disc - 1, use, to, from);
            }
        }

        public static void Main()
        {
            Console.Clear();
            Console.WriteLine("Enter number of discs?");
            var numberOfDiscs = int.Parse(Console.ReadLine());

            MoveTower(numberOfDiscs, 1, 3, 2);

            Console.Write("Number of moves ");
            Console.WriteLine(NumberOfMoves);
        }
    }
}
