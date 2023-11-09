namespace Mines
{
    public class Square(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public int AdjacentMines { get; internal set; } = 0;

        public bool Mined { get; internal set; } = false;
        public bool Revealed { get; internal set; } = false;
        public bool Flagged { get; internal set; } = false;

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
