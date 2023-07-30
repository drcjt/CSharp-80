namespace Chess
{
    public readonly struct Move
    {
        public readonly byte FromSquare;
        public readonly byte ToSquare;

        public Move(int fromIndex, int toIndex)
        {
            FromSquare = (byte)fromIndex;
            ToSquare = (byte)toIndex;
        }

        public Move(string fromSquare, string toSquare)
        {
            FromSquare = Notation.ToSquare(fromSquare);
            ToSquare = Notation.ToSquare(toSquare);
        }

        public Move(string uciMoveNotation)
        {
            string fromSquare = uciMoveNotation.Substring(0, 2);
            string toSquare = uciMoveNotation.Substring(2, 2);

            FromSquare = Notation.ToSquare(fromSquare);
            ToSquare = Notation.ToSquare(toSquare);
        }
    }
}
