using System;
using System.Threading;

namespace Mines
{
    internal class Game
    {
        private readonly int _columns;
        private readonly int _rows;
        private readonly int _mines;
        private readonly Board _board;

        private int _flagged;
        private int _moves;
        private bool _revealMines;

        private const string Playing = "  :)  ";
        private const string Lost = "  :(  ";
        private const string Won = "(\"#_#)";

        public Game(int columns, int rows, int mines)
        {
            _columns = columns + 2; 
            _rows = rows + 2;
            _mines = mines;

            _board = new Board(_columns, _rows, mines);
        }

        public void Play()
        {
            int cursorX = 1;
            int cursorY = 1;
            const char cursorChar = (char)0x8f;

            DisplayEmptyBoard();


            char cellCharacter = VisualizeCurrentState(cursorX, cursorY);
            DisplayCell(cursorChar, cursorX, cursorY);

            int completedMoves = ((_columns - 2) * (_rows - 2)) - _mines;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo ki = Console.ReadKey(true);
                    int kc = ki.KeyChar;

                    if (kc != 10 && kc != 91 && kc != 9 && kc != 8 && kc != 81 && kc != 114 && kc != 102)
                        continue;

                    DisplayCell(cellCharacter, cursorX, cursorY);

                    switch (kc)
                    {
                        case 10: if (cursorY < _rows - 2) cursorY++; break;
                        case 91: if (cursorY > 1) cursorY--; break;
                        case 9: if (cursorX < _columns - 2) cursorX++; break;
                        case 8: if (cursorX > 1) cursorX--; break;
                        case 81: Environment.Exit(0); break;
                        case 114: Reveal(cursorX, cursorY); break;
                        case 102: Flag(cursorX, cursorY); break;
                    }

                    // Game over
                    if (_moves == completedMoves || _revealMines)
                    {
                        RevealMines();

                        Console.SetConsoleCursorPosition((sbyte)(_columns - 4), 0);
                        Console.Write(_revealMines ? Lost : Won);

                        Thread.Sleep(500);

                        while (!Console.KeyAvailable);

                        Console.Clear();

                        _board.Reset();
                        DisplayEmptyBoard();

                        cursorX = 1;
                        cursorY = 1;

                        _flagged = 0;
                        _moves = 0;
                        _revealMines = false;
                    }

                    cellCharacter = VisualizeCurrentState(cursorX, cursorY);
                    DisplayCell(cursorChar, cursorX, cursorY);
                }
            }
        }

        private static void DisplayCell(char cell, int x, int y)
        {
            Console.SetConsoleCursorPosition((sbyte)(x * 2), (sbyte)y);
            Console.Write(cell);
        }

        private static void DisplayCell(Square square) => DisplayCell(square.AsChar(), square.X, square.Y);

        private char VisualizeCurrentState(int x, int y) => _board.Get(x, y).AsChar();

        public void DisplayEmptyBoard()
        {
            Console.Clear();
            Console.WriteLine(_mines);
            for (var y = 1; y < _rows - 1; y++)
            {
                Console.Write("+ ");
                for (var x = 1; x < _columns - 1; x++)
                {
                    Console.Write("  ");
                }
                Console.WriteLine("+");
            }

            Console.SetConsoleCursorPosition((sbyte)(_columns - 4), 0);
            Console.Write(Playing);

        }

        private void RevealMines()
        {
            for (int x = 1; x < _columns - 1; x++)
            {
                for (int y = 1; y < _rows - 1; y++)
                {
                    var square = _board.Get(x, y);
                    if (square.Mined)
                    {
                        DisplayCell('*', square.X, square.Y);
                    }
                }
            }
        }

        private void Flag(int x, int y)
        {
            var square = _board.Get(x, y);
            square.Flagged = !square.Flagged;

            _flagged += square.Flagged ? 1 : -1;

            Console.SetConsoleCursorPosition(0, 0);
            Console.Write("   ");
            Console.SetConsoleCursorPosition(0, 0);
            Console.Write(_mines - _flagged);
        }

        private void Reveal(int x, int y)
        {
            if (x == 0 || y == 0 || x == (_columns - 1) || y == (_rows - 1))
                return;

            var square = _board.Get(x, y);

            if (square.Mined)
            {
                /*
                if (_moves == 0)
                {
                    _board.MoveMine(square);
                }
                else
                */
                {
                    _revealMines = true;
                    return;
                }
            }

            if (square.Revealed)
                return;

            _moves++;

            square.Revealed = true;
            DisplayCell(square);

            if (square.AdjacentMines == 0)
            {
                Reveal(x - 1, y - 1);
                Reveal(x - 1, y);
                Reveal(x - 1, y + 1);
                Reveal(x, y - 1);
                Reveal(x, y + 1);
                Reveal(x + 1, y - 1);
                Reveal(x + 1, y);
                Reveal(x + 1, y + 1);
            }
        }
    }
}
