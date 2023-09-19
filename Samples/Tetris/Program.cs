using System;

namespace Tetris
{
    public static class Program
    {
        public static void Main()
        {
            Console.Clear();

            var game = new Game();
            game.Play();
        }
    }
}
