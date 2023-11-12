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

            char cellCharacter = _board.Get(cursorX, cursorY).AsChar();
            DisplayCell(cursorChar, cursorX, cursorY);

            int completedMoves = ((_columns - 2) * (_rows - 2)) - _mines;

            bool playing = true;
            while (playing)
            {
                int kc = GetKeyCode();

                if (IsValidKey(kc))
                {
                    DisplayCell(cellCharacter, cursorX, cursorY);

                    switch (kc)
                    {
                        case 10: cursorY = Math.Min(cursorY + 1, _rows - 2); break;
                        case 91: cursorY = Math.Max(cursorY - 1, 1); break;
                        case 9: cursorX = Math.Min(cursorX + 1, _columns - 2); break;
                        case 8: cursorX = Math.Max(cursorX - 1, 1); break;
                        case 81: playing = false; break;
                        case 114: Reveal(cursorX, cursorY); break;
                        case 102: Flag(cursorX, cursorY); break;
                    }

                    // Game over
                    if (playing && (_moves == completedMoves || _revealMines))
                    {
                        GameOver();

                        cursorX = 1;
                        cursorY = 1;
                    }

                    cellCharacter = _board.Get(cursorX, cursorY).AsChar();
                    DisplayCell(cursorChar, cursorX, cursorY);
                }
            }
        }

        private void GameOver()
        {
            RevealMines();

            Console.SetCursorPosition(_columns - 4, 0);
            Console.Write(_revealMines ? Lost : Won);

            Thread.Sleep(500);

            WaitForKeyPress();

            Console.Clear();

            _board.Reset();
            DisplayEmptyBoard();

            _flagged = 0;
            _moves = 0;
            _revealMines = false;
        }

        private static void WaitForKeyPress()
        {
            while (!Console.KeyAvailable)
            {
                // Wait for keypress
            }
        }

        private static int GetKeyCode()
        {
            WaitForKeyPress();

            // Get details of key
            ConsoleKeyInfo ki = Console.ReadKey(true);
            int kc = ki.KeyChar;
            return kc;
        }

        private static bool IsValidKey(int kc) => kc == 10 || kc == 91 || kc == 9 || kc == 8 || kc == 81 || kc == 114 || kc == 102;

        private static void DisplayCell(char cell, int x, int y)
        {
            Console.SetCursorPosition(x * 2, y);
            Console.Write(cell);
        }

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

            Console.SetCursorPosition(_columns - 4, 0);
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

            Console.SetCursorPosition(0, 0);
            Console.Write("   ");
            Console.SetCursorPosition(0, 0);
            Console.Write(_mines - _flagged);
        }

        private void Reveal(int x, int y)
        {
            if (x == 0 || y == 0 || x == (_columns - 1) || y == (_rows - 1))
                return;

            var square = _board.Get(x, y);
            if (square.Revealed)
                return;

            if (square.Mined)
            {
                // TODO: if _moves == 0 then move mine

                _revealMines = true;
                return;
            }

            _moves++;

            square.Revealed = true;
            DisplayCell(square.AsChar(), square.X, square.Y);

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
