using System;

namespace Life
{
    public static class Life
    {
        private static CellMap? CurrentMap;

        public static unsafe void Main()
        {
            var rows = Console.WindowHeight - 1;
            var columns = Console.WindowWidth - 1;

            byte* currentCells = stackalloc byte[rows * columns];
            CurrentMap = new CellMap(rows, columns, currentCells);

            Console.Clear();

            CurrentMap.Init();

            for (var generation = 1; generation < 50; generation++)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("Generation : ");
                Console.Write(generation);

                var startTime = Environment.GetDateTime();

                CurrentMap.NextGeneration();

                var endTime = Environment.GetDateTime();
                var elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

                Console.SetCursorPosition(25, 0);
                Console.Write("Time: ");
                Console.Write(elapsedTime);
            }
        }
    }
}