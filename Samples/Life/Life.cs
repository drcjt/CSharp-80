using System;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Life
{
    public static class Life
    {
        public static unsafe void Main()
        {
            var rows = Console.WindowHeight - 1;
            var columns = Console.WindowWidth - 1;

            byte* currentCells = stackalloc byte[rows * columns];
            byte* workerCells = stackalloc byte[rows * columns];
            var currentMap = new CellMap(rows, columns, currentCells, workerCells);

            Console.Clear();

            currentMap.Init();

            var startTime = DateTime.Now;

            for (var generation = 1; generation < 50; generation++)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("Generation : ");
                Console.Write(generation);

                var nextGenerationStartTime = DateTime.Now;

                currentMap.NextGeneration();

                var nextGenerationEndTime = DateTime.Now;
                var elapsedTime = nextGenerationEndTime.TotalSeconds - nextGenerationStartTime.TotalSeconds;

                Console.SetCursorPosition(25, 0);
                Console.Write("Time: ");
                Console.Write(elapsedTime);
            }

            var endTime = DateTime.Now;
            var overallTime = endTime.TotalSeconds - startTime.TotalSeconds;
            Console.SetCursorPosition(25, 0);
            Console.Write("Overall Time: ");
            Console.Write(overallTime);
        }
    }
}