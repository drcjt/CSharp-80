using System;

namespace Mines
{
    public class Board
    {
        private readonly Square[] _squares;
        private readonly int _columns;
        private readonly int _rows;
        private readonly int _mines;

        public Board(int columns, int rows, int mines)
        {
            _columns = columns;
            _rows = rows;
            _mines = mines;

            _squares = new Square[columns * rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    _squares[(_columns * y) + x] = new Square(x, y);
                }
            }

            LayMines();
        }

        public void LayMines()
        {
            // Populate mines
            var random = new LehmerRandom(Environment.TickCount);
            var placed = 0;

            while (placed < _mines)
            {
                var x = random.Next(_columns - 2);
                var y = random.Next(_rows - 2);

                var tile = Get(x + 1, y + 1);

                if (!tile.Mined)
                {
                    tile.Mined = true;
                    placed++;
                }
            }

            for (int x = 1; x < _columns - 1; x++)
            {
                for (int y = 1; y < _rows - 1; y++)
                {
                    CountAdjacentMines(Get(x, y));
                }
            }
        }

        public void Reset()
        {
            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    Get(x, y).Reset();
                }
            }

            LayMines();
        }

        public void CountAdjacentMines(Square tile)
        {
            int count = 0;

            for (int ny = tile.Y - 1; ny <= tile.Y + 1; ny++)
            {
                for (int nx = tile.X - 1; nx <= tile.X + 1; nx++)
                {
                    if (Get(nx, ny).Mined)
                    {
                        count++;
                    }
                }
            }

            tile.AdjacentMines = count;
        }

        public void MoveMine(Square originalTile)
        {
            for (var newBombY = 1; newBombY < _rows - 1; newBombY++)
            {
                for (var newBombX = 1; newBombX < _columns - 1; newBombX++)
                {
                    var tile = Get(newBombX, newBombY);
                    if (!tile.Mined)
                    {
                        originalTile.Mined = false;
                        tile.Mined = true;

                        // TODO: Is this correct - does moving a mine only affect the adjacent count
                        // directly for the squares being altered??

                        CountAdjacentMines(tile);
                        CountAdjacentMines(originalTile);
                        return;
                    }
                }
            }
        }

        public Square Get(int x, int y) => _squares[(_columns * y) + x];
    }
}
