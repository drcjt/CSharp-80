using System;
using System.Threading;

namespace Tetris
{
    internal class Game
    {
        const int FrameWidth = 10;
        static readonly int FrameHeight = Console.WindowHeight - 1;

        private bool[] board;
        private bool[] currentPiece;
        private bool[] nextPiece;

        private int currentPieceX;
        private int currentPieceY;

        public void Play()
        {
            board = new bool[FrameWidth * FrameHeight];
            currentPiece = new bool[4 * 4];
            nextPiece = new bool[4 * 4];

            for (var i = 0; i < board.Length; i++) board[i] = false;
            for (var i = 0; i < currentPiece.Length; i++) currentPiece[i] = false;
            for (var i = 0; i < nextPiece.Length; i++) nextPiece[i] = false;

            currentPiece[0] = true;
            currentPiece[1] = true;
            currentPiece[2] = true;
            currentPiece[3] = true;

            currentPieceX = 3;
            currentPieceY = 0;

            DrawFrame();

            var ch = '-';

            while (true)
            {
                DrawCurrentPiece();

                var pieceXOffset = 0;

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo ki = Console.ReadKey(true);
                    int kc = ki.KeyChar;
                    switch (kc)
                    {
                        case 10: break; // down
                        case 91: break; // up
                        case 8:
                            pieceXOffset = -1;
                            break; // left
                        case 9:
                            pieceXOffset = +1;
                            break; // right
                        case 81:
                            Environment.Exit(0);
                            break; // Q, quit
                    }
                }

                Console.SetConsoleCursorPosition((sbyte)(Console.WindowWidth - 1), 0);
                Console.Write(ch);
                ch = ch == '-' ? '|' : '-';

                // Wait a bit
                Thread.Sleep(500);

                currentPieceY++;
                if (CollisionCheck())
                {
                    // Move piece back
                    currentPieceY--;

                    // Make piece part of board
                    for (var row = 0; row < 4; row++)
                    {
                        for (var col = 0; col < 4; col++)
                        {
                            board[((row + currentPieceY) * 4) + (col + currentPieceX)] = currentPiece[(row * 4) + col];
                        }
                    }

                    // Reset piece to first row to start over.
                    currentPieceY = 0;
                }
                else
                {
                    currentPieceY--;

                    // Erase piece
                    DrawCurrentPiece(true);

                    // Move piece
                    currentPieceY++;
                    if (pieceXOffset != 0)
                    {
                        currentPieceX += pieceXOffset;
                        if (currentPieceX < 0)
                        {
                            currentPieceX = 0;
                        }
                        if (currentPieceX >= FrameWidth)
                        {
                            currentPieceX = FrameWidth - 1;
                        }
                    }
                }
            }
        }

        private bool CollisionCheck()
        {
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    var ch = currentPiece[(row * 4) + col];
                    if (ch)
                    {
                        if (currentPieceY + row > FrameHeight)
                        {
                            return true;
                        }
                        var boardCell = board[((row + currentPieceY) * 4) + (col + currentPieceX)];
                        if (ch & boardCell)
                        {
                            return true;
                        }
                    }   
                }
            }

            return false;
        }

        private void DrawCurrentPiece(bool erase = false)
        {
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    var ch = currentPiece[(row * 4) + col];
                    if (ch)
                    {
                        Console.SetConsoleCursorPosition((sbyte)(currentPieceX + 1 + col), (sbyte)(currentPieceY + row));
                        Console.Write(erase ? ' ' : 'O');
                    }
                }
            }
        }

        private static void DrawFrame()
        {
            for (sbyte row = 0; row < FrameHeight; row++)
            {
                Console.SetConsoleCursorPosition(0, row);
                Console.Write("|");
                Console.SetConsoleCursorPosition(FrameWidth + 1, row);
                Console.Write("|");
            }
            Console.SetConsoleCursorPosition(0, (sbyte)FrameHeight);
            for (var column = 0; column < FrameWidth + 2; column++)
            {
                Console.Write("-");
            }

            const int RightOfFrameColumn = FrameWidth + 3;

            Console.SetConsoleCursorPosition(RightOfFrameColumn, 0);
            Console.Write("Next");

            Console.SetConsoleCursorPosition(RightOfFrameColumn, 6);
            Console.Write("Score");

            Console.SetConsoleCursorPosition(RightOfFrameColumn, 9);
            Console.Write("Lines");

            Console.SetConsoleCursorPosition(RightOfFrameColumn, 12);
            Console.Write("Level");
        }
    }
}
