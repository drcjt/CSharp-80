namespace Mines
{
    public class Square
    {
        public int X { get; }
        public int Y { get; }
        public int AdjacentMines { get; internal set; } = 0;

        public bool Mined { get; internal set; } = false;
        public bool Revealed { get; internal set; } = false;
        public bool Flagged { get; internal set; } = false;

        public Square(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Reset()
        {
            AdjacentMines = 0;
            Mined = false;
            Revealed = false;
            Flagged = false;
        }

        public char AsChar()
        {
            if (Flagged) return 'F';
            if (!Revealed) return ' ';
            if (Mined) return '*';
            return (char)('0' + AdjacentMines);
        }
    }
}
