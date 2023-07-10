using System;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Matrix
{
    public enum RainState
    {
        None,
        Raining,
        StopRaining
    }
    public struct RainColumn
    {
        public RainState State { get; set; }
        public int Row { get; set; }
        public int Speed { get; set; }
    }

    public static unsafe class Program
    {
        static byte* address = (byte*)0;
        private static unsafe char RandomChar() => (char)((*address++ & 63) + '!');
        private static unsafe int RandomSpeed() => (*address & 2) + 1;

        static readonly int Height = Console.WindowHeight;
        static readonly int Width = Console.WindowWidth - 1;
        
        const char LeadingCharacter = (char)0x8f;
        const char Blank = ' ';

        private static readonly Random _random = new Random();

        public unsafe static void Main()
        {
            RainColumn* rainColumns = stackalloc RainColumn[Width];
            InitialiseRainColumns(rainColumns);

            Console.Clear();

            var quit = false;
            while (!quit)
            {
                // Quit if user presses Q
                if (Console.KeyAvailable)
                {
                    var cki = Console.ReadKey(true);
                    quit = cki.KeyChar == 81;
                }

                PickColumnToRain(rainColumns);

                // Use byte* to generate 16 bit arithmetic for loop
                for (byte* pcol = (byte*)0; pcol < (byte*)Width; pcol++)
                {
                    switch (rainColumns[(int)pcol].State)
                    {
                        case RainState.None: continue;
                        case RainState.Raining:
                            Rain(rainColumns, (int)pcol);
                            break;
                        case RainState.StopRaining:
                            StopRain(rainColumns, (int)pcol);
                            break;
                    }
                }
            }
        }

        private static void StopRain(RainColumn* rainColumns, int col)
        {
            for (int i = 0; i < rainColumns[col].Speed; i++)
            {
                int row = rainColumns[col].Row;
                WriteAtCursorPosition(col, row, Blank);
                row++;
                if (row == Height)
                {
                    rainColumns[col].State = RainState.None;
                    rainColumns[col].Row = 0;
                    break;
                }
                rainColumns[col].Row = row;
            }
        }

        private static void Rain(RainColumn* rainColumns, int col)
        {
            for (int i = 0; i < rainColumns[col].Speed; i++)
            {
                var row = rainColumns[col].Row;
                WriteAtCursorPosition(col, row, RandomChar());

                row++;
                if (row == Height)
                {
                    rainColumns[col].State = RainState.StopRaining;
                    rainColumns[col].Row = 0;
                    break;
                }
                else
                {
                    WriteAtCursorPosition(col, row, LeadingCharacter);
                }

                rainColumns[col].Row = row;
            }
        }

        private static void PickColumnToRain(RainColumn* rainColumns)
        {
            var column = _random.Next(Width);
            if (rainColumns[column].State == RainState.None)
            {
                rainColumns[column].State = RainState.Raining;
                rainColumns[column].Speed = RandomSpeed();
            }
        }

        private static void InitialiseRainColumns(RainColumn* rainColumns)
        {
            for (int index = 0; index < Width; index++)
            {
                rainColumns[index].State = RainState.None;
                rainColumns[index].Row = 0;
                rainColumns[index].Speed = 2;
            }
        }
        private static void WriteAtCursorPosition(int col, int row, char ch)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(ch);
        }
    }
}
