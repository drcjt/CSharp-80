using System;

namespace Mines
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Beginner (1), Intermediate (2), or Expert (3)?");
            var level = int.Parse(Console.ReadLine());

            if (level == 1)
            {
                var game = new Game(8, 8, 10);
                game.Play();
            } 
            else if (level == 2)
            {
                var game = new Game(11, 11, 24);
                game.Play();
            }
            else if (level == 3)
            {
                var game = new Game(14, 14, 45);
                game.Play();
            }
        }
    }
}
