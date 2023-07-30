namespace Chess
{
    public enum Color
    {
        Black,
        White
    }

    public enum Piece : byte
    {
        // 1st bit indicates if piece or not
        None = 0,

        // 2nd bit indicates color of piece
        Black = 1,
        White = 3,

        // 3rd and more bits indicate type of piece
        Pawn = 4,
        Knight = 8,
        Bishop = 12,
        Rook = 16,
        Queen = 20,
        King = 24,

        WhitePawn = White + Pawn,
        WhiteKnight = White + Knight,
        WhiteBishop = White + Bishop,
        WhiteRook = White + Rook,
        WhiteQueen = White + Queen,
        WhiteKing = White + King,

        BlackPawn = Black + Pawn,
        BlackKnight = Black + Knight,
        BlackBishop = Black + Bishop,
        BlackRook = Black + Rook,
        BlackQueen = Black + Queen,
        BlackKing = Black + King,

        ColorMask = 0b00000011,
        TypeMask = 0b01111100
    }

    public static class Pieces
    {
        public static Color Color(Piece piece) => (Color)((piece & Piece.ColorMask));

        public static bool IsWhite(Piece piece) => (piece & Piece.ColorMask) == Piece.White;
        public static bool IsBlack(Piece piece) => (piece & Piece.ColorMask) == Piece.Black;
    }
}
