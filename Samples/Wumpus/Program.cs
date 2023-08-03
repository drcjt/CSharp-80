using Wumpus;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace MiniBCL
{
    public static class Program
    {
        public static void Main()
        {
            var game = new Game();
            game.Play();
        }
    }
}
