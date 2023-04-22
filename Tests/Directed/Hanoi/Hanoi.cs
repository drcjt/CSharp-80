namespace Hanoi
{
    public static class Tests
    {
        private static int numberOfMoves;

        static void MoveTower(int disc, int from, int to, int use)
        {
            if (disc > 0)
            {
                numberOfMoves++;
                MoveTower(disc - 1, from, use, to);
                MoveTower(disc - 1, use, to, from);
            }
        }

        public static int Main()
        {
            int iterations = 5;
            int numberOfDiscs = 12;

            numberOfMoves = 0;
            while (iterations > 0)
            {
                iterations--;
                MoveTower(numberOfDiscs, 1, 3, 2);
            }
            if (numberOfMoves != 20475)
                return 1;
            return 0;
        }
    }
}