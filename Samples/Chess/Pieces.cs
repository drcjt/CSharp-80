using System;

namespace Chess
{
    public enum Color
    {
        Black,
        White
    }

    public enum Pieces : byte
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

        WhitePawn = White | Pawn,
        WhiteKnight = White | Knight,
        WhiteBishop = White | Bishop,
        WhiteRook = White | Rook,
        WhiteQueen = White | Queen,
        WhiteKing = White | King,

        BlackPawn = Black | Pawn,
        BlackKnight = Black | Knight,
        BlackBishop = Black | Bishop,
        BlackRook = Black | Rook,
        BlackQueen = Black | Queen,
        BlackKing = Black | King,

        ColorMask = 0b00000011,
        TypeMask = Pawn | Knight | Bishop | Rook | Queen | King,
    }

    public static class PiecesExtensions
    {
        public static Color Color(this Pieces piece) => (Color)(piece & Pieces.ColorMask);

        public static bool IsWhite(this Pieces piece) => (piece & Pieces.ColorMask) == Pieces.White;
        public static bool IsBlack(this Pieces piece) => (piece & Pieces.ColorMask) == Pieces.Black;
    }
}
