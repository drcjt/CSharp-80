namespace Chess
{
    public static class Notation
    {
        public static char ToChar(Pieces piece)
        {
            return piece switch
            {
                Pieces.WhitePawn => 'P',
                Pieces.WhiteKnight => 'N',
                Pieces.WhiteBishop => 'B',
                Pieces.WhiteRook => 'R',
                Pieces.WhiteQueen => 'Q',
                Pieces.WhiteKing => 'K',
                Pieces.BlackPawn => 'p',
                Pieces.BlackKnight => 'n',
                Pieces.BlackBishop => 'b',
                Pieces.BlackRook => 'r',
                Pieces.BlackQueen => 'q',
                Pieces.BlackKing => 'k',
                _ => '.',
            };
        }

        public static Pieces ToPiece(char ch)
        {
            return ch switch
            {
                'P' => Pieces.WhitePawn,
                'N' => Pieces.WhiteKnight,
                'B' => Pieces.WhiteBishop,
                'R' => Pieces.WhiteRook,
                'Q' => Pieces.WhiteQueen,
                'K' => Pieces.WhiteKing,
                'p' => Pieces.BlackPawn,
                'n' => Pieces.BlackKnight,
                'b' => Pieces.BlackBishop,
                'r' => Pieces.BlackRook,
                'q' => Pieces.BlackQueen,
                'k' => Pieces.BlackKing,
                _ => Pieces.None,
                // _ => throw new ArgumentException($"Unknown piece character {ch}");
            };
        }

        public static string ToSquareName(byte squareIndex)
        {
            int rank = Board.Rank(squareIndex);
            int file = Board.File(squareIndex);

            char[] squareNotation = new char[2] { (char)('a' + file), (char)('1' + rank) };
            return new string(squareNotation);
        }

        public static byte ToSquare(string squareNotation)
        {
            int file = squareNotation[0] - 'a';
            int rank= squareNotation[1] - '1';
            int index = Board.Square(rank, file);

            return (byte)index;
        }
    }
}
