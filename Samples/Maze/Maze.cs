using System;
using System.Drawing;

namespace Maze
{
    [Flags]
    internal enum CellState : byte
    {
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8,

        Visited = 128,
    }

    /// <summary>
    /// Generates a maze of specified dimensions using a randomized algorithm.
    /// using recursive backtracking.
    /// </summary>
    public static class MazeGenerator
    {
        // Directions, left, up, right, down
        private static readonly CellState[,] Maze = new CellState[Width, Height];
        private static readonly Random Rand = new();
        private const int Width = Graphics.ScreenWidth / 2;
        private const int Height = Graphics.ScreenHeight / 2;

        public static void Main()
        {
            DrawGrid();
            GenerateMaze(0, 0);

            while (!Console.KeyAvailable) 
            {
                // Wait for any key press
            }
        }

        // Permutations of all possible directions from one cell to another
        // encoded into one byte, 2 bits = 1 direction, so 00 01 10 11 = 0, 1, 2, 3 directions
        private static readonly byte[] _orders =
        [
            27, 30, 39, 45, 54, 57, 75, 78, 99, 108, 114, 120, 135, 141, 147, 156, 177, 180, 198, 201, 210, 216, 225, 228
        ];

        private static void GenerateMaze(int x, int y)
        {
            Maze[x, y] |= CellState.Visited; // Mark the cell as visited

            // Pick an order to visit the neighboring cells
            byte order = _orders[Rand.Next(_orders.Length)];
            for (nuint i = 0; i < 4; i++)
            {
                // Get the direction from the order
                byte direction = (byte)(order & 3);

                // Shift the order to get the next direction
                order >>= 2;

                if (CellInBoundsAndUnvisited(x, y, direction))
                {
                    CarveWalls(x, y, direction);
                    GenerateMaze(x + DeltaX(direction), y + DeltaY(direction));
                }
            }
        }

        private static bool CellInBoundsAndUnvisited(int x, int y, byte direction)
        {
            x += DeltaX(direction);
            y += DeltaY(direction);

            return (x >= 0 && y >= 0 && x < Width && y < Height) 
                && ((Maze[x, y] & CellState.Visited) == 0);
        }

        private static void CarveWalls(int x, int y, byte direction)
        {
            Graphics.SetPixel((x * 2) + 1 + DeltaX(direction), (y * 2) + 1 + DeltaY(direction), Color.Black);

            Maze[x, y] |= (CellState)(1 << direction); // Mark the direction as open

            x += DeltaX(direction);
            y += DeltaY(direction);

            Maze[x, y] |= (CellState)(1 << Opposite(direction)); // Mark the opposite direction as open
        }

        private static byte Opposite(byte direction) => (byte)((direction + 2) % 4);

        private static sbyte DeltaX(byte direction) => direction switch
        {
            0 => -1,
            1 => 0,
            2 => 1,
            3 => 0,
            _ => throw new NotImplementedException()
        };

        private static sbyte DeltaY(byte direction) => direction switch
        {
            0 => 0,
            1 => -1,
            2 => 0,
            3 => 1,
            _ => throw new NotImplementedException()
        };

        public static void DrawGrid()
        {
            Console.Clear();
            for (int x = 0; x < (Width * 2) + 1; x += 2)
            {
                Graphics.DrawLine(Pens.White, x, 0, x, Height * 2);
            }
            for (int y = 0; y < (Height * 2) + 1; y += 2)
            {
                Graphics.DrawLine(Pens.White, 0, y, Width * 2, y);
            }
        }
    }
}