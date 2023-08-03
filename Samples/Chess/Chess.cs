using System;
using System.Threading;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace Chess
{
    public static class Chess
    {
        public static void Main()
        {
            Console.Clear();

            var board = new Board();

            string startingPosition = 
                "rnbqkbnr" + 
                "pppppppp" + 
                "........" +
                "........" +
                "........" +
                "........" +
                "PPPPPPPP" +
                "RNBQKBNR";

            board.SetupPosition(startingPosition);

            do
            {
                Print(board);

                var command = Console.ReadLine();

                if (command[0] == 'Q')
                {
                    break;
                }

                var move = new Move(command);

                if (board.IsMovePlayable(move))
                {
                    board.PlayMove(move);
                }
                else
                {
                    Console.WriteLine("Invalid move");
                    Thread.Sleep(1000);
                }
            }
            while (true);
        }

        public static void Print(Board board)
        {
            Console.SetConsoleCursorPosition(0, 0);
            Console.WriteLine("   A B C D E F G H");
            Console.WriteLine(" .----------------.");
            for (int rank = 7; rank >= 0; rank--)
            {
                Console.Write(rank + 1); //ranks aren't zero-indexed
                Console.Write("| ");

                for (int file = 0; file < 8; file++)
                {
                    var piece = board[rank, file];
                    Console.Write(Notation.ToChar(piece));
                    Console.Write(' ');
                }

                Console.Write("|");
                Console.WriteLine(rank + 1); //ranks aren't zero-indexed
            }
            Console.WriteLine(" '----------------'");
            Console.WriteLine("   A B C D E F G H");
        }
    }
}
