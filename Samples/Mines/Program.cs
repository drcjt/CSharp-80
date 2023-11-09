namespace Mines
{
    public static class Program
    {
        public static void Main()
        {
            // Beginner
            const int Width = 8;
            const int Height = 8;
            const int Bombs = 10;

            // Intermediate
            /*
            const int Width = 9;
            const int Height = 9;
            const int Bombs = 10;
            */

            // Expert
            /*
            const int Width = 30;
            const int Height = 15; // Should be 15 but can't fit 16 and status row on Trs-80 screen
            const int Bombs = 99;
             */

            var game = new Game(Width, Height, Bombs);
            game.Play();
        }
    }
}
