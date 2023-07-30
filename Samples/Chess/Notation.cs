namespace Chess
{
    public static class Notation
    {
        public static char ToChar(Piece piece)
        {
            return piece switch
            {
                Piece.WhitePawn => 'P',
                Piece.WhiteKnight => 'N',
                Piece.WhiteBishop => 'B',
                Piece.WhiteRook => 'R',
                Piece.WhiteQueen => 'Q',
                Piece.WhiteKing => 'K',
                Piece.BlackPawn => 'p',
                Piece.BlackKnight => 'n',
                Piece.BlackBishop => 'b',
                Piece.BlackRook => 'r',
                Piece.BlackQueen => 'q',
                Piece.BlackKing => 'k',
                _ => '.',
            };
        }

        public static Piece ToPiece(char ch)
        {
            return ch switch
            {
                'P' => Piece.WhitePawn,
                'N' => Piece.WhiteKnight,
                'B' => Piece.WhiteBishop,
                'R' => Piece.WhiteRook,
                'Q' => Piece.WhiteQueen,
                'K' => Piece.WhiteKing,
                'p' => Piece.BlackPawn,
                'n' => Piece.BlackKnight,
                'b' => Piece.BlackBishop,
                'r' => Piece.BlackRook,
                'q' => Piece.BlackQueen,
                'k' => Piece.BlackKing,
                _ => Piece.None,
                // _ => throw new ArgumentException($"Unknown piece character {ch}");
            };
        }

        public static string ToSquareName(byte squareIndex)
        {
            int rank = squareIndex / 8;
            int file = squareIndex % 8;

            char[] squareNotation = new char[2] { (char)('a' + file), (char)('1' + rank) };
            return new string(squareNotation);
        }

        public static byte ToSquare(string squareNotation)
        {
            int file = squareNotation[0] - 'a';
            int rank= squareNotation[1] - '1';
            int index = rank * 8 + file;

            return (byte)index;
        }
    }
}
