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
        public RainState state;
        public int row;
        public int speed;
    }

    public static unsafe class Program
    {
        static byte* address = (byte*)0;
        private static unsafe char RandomChar() => (char)((*address++ & 63) + '!');
        private static unsafe int RandomSpeed() => (*address & 2) + 1;

        static int Height;
        static int Width;
        
        const char LeadingCharacter = (char)0x8f;
        const char Blank = ' ';

        private static Random _random;

        public unsafe static void Main()
        {
            _random = new Random(); // NOSONAR

            Height = Console.WindowHeight;
            Width = Console.WindowWidth - 1;

            RainColumn* rainColumns = stackalloc RainColumn[Width];
            InitialiseRainColumns(rainColumns);

            Console.Clear();

            while (true)
            {
                PickColumnToRain(rainColumns);

                // Use byte* to generate 16 bit arithmetic for loop
                for (byte* pcol = (byte*)0; pcol < (byte*)Width; pcol++)
                {
                    switch (rainColumns[(int)pcol].state)
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
            for (int i = 0; i < rainColumns[col].speed; i++)
            {
                int row = rainColumns[col].row;
                WriteAtCursorPosition(col, row, Blank);
                row++;
                if (row == Height)
                {
                    rainColumns[col].state = RainState.None;
                    rainColumns[col].row = 0;
                    break;
                }
                rainColumns[col].row = row;
            }
        }

        private static void Rain(RainColumn* rainColumns, int col)
        {
            for (int i = 0; i < rainColumns[col].speed; i++)
            {
                var row = rainColumns[col].row;
                WriteAtCursorPosition(col, row, RandomChar());

                row++;
                if (row == Height)
                {
                    rainColumns[col].state = RainState.StopRaining;
                    rainColumns[col].row = 0;
                    break;
                }
                else
                {
                    WriteAtCursorPosition(col, row, LeadingCharacter);
                }

                rainColumns[col].row = row;
            }
        }

        private static void PickColumnToRain(RainColumn* rainColumns)
        {
            var column = _random.Next(Width);
            if (rainColumns[column].state == RainState.None)
            {
                rainColumns[column].state = RainState.Raining;
                rainColumns[column].speed = RandomSpeed();
            }
        }

        private static void InitialiseRainColumns(RainColumn* rainColumns)
        {
            for (int index = 0; index < Width; index++)
            {
                rainColumns[index].state = RainState.None;
                rainColumns[index].row = 0;
                rainColumns[index].speed = 2;
            }
        }
        private static void WriteAtCursorPosition(int col, int row, char ch)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(ch);
        }
    }
}
