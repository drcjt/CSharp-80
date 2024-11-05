namespace Wordle
{
    public static class Program
    {
        public static void Main()
        {
            var game = new Game(["apple", "bench", "chair", "dance", "earth", "faith", "giant", "happy"]);
            game.Play();
        }
    }
}