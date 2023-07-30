using System;

namespace Chess
{
    public class Board
    {
        private readonly Piece[] _state = new Piece[64];

        public Piece this[int square]
        {
            get => _state[square];
            private set
            {
                _state[square] = value;
            }
        }

        public Piece this[int rank, int file] => _state[rank * 8 + file];

        public void SetupPosition(string fen)
        {
            // Array.Clear(_state, 0, 64);
            for (int i = 0; i < _state.Length; i++)
            {
                _state[i] = 0;
            }

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    char piece = fen[rank * 8 + file];
                    _state[rank * 8 + file] = Notation.ToPiece(piece);
                }
            }
        }

        public void PlayMove(Move move)
        {
            Piece movingPiece = this[move.FromSquare];

            this[move.ToSquare] = movingPiece;
            this[move.FromSquare] = Piece.None;
        }

        private static int Rank(int square) => square / 8;
        private static int File(int square) => square % 8;
        private static int Up(int square, int steps = 1) => square + steps * 8;
        private static int Down(int square, int steps = 1) => square - steps * 8;

        public bool IsMovePlayable(Move move)
        {
            Piece movingPiece = this[move.FromSquare];
            switch (movingPiece)
            {
                case Piece.BlackPawn:
                    return IsBlackPawnMovePlayable(move);

                case Piece.WhitePawn:
                    return IsWhitePawnMovePlayable(move);

                case Piece.BlackRook:
                case Piece.WhiteRook:
                    return IsRookMovePlayable(move);
            }

            return false;
        }

        private bool IsRookMovePlayable(Move move)
        {
            // Rook moves in straight lines

            if (Rank(move.FromSquare) == Rank(move.ToSquare))
            {
                var lowerFile = Math.Min(File(move.FromSquare), File(move.ToSquare));
                var higherFile = Math.Max(File(move.FromSquare), File(move.ToSquare));
                for (int file = lowerFile; file < higherFile; file++)
                {
                    if (file != File(move.FromSquare) && this[Rank(move.FromSquare), file] != Piece.None)
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (File(move.FromSquare) == File(move.ToSquare))
            {
                var lowerRank = Math.Min(Rank(move.FromSquare), Rank(move.ToSquare));
                var higherRank = Math.Max(Rank(move.FromSquare), Rank(move.ToSquare));
                for (int rank = lowerRank; rank < higherRank; rank++)
                {
                    if (rank != Rank(move.FromSquare) && this[rank, File(move.FromSquare)] != Piece.None)
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        private bool IsWhitePawnMovePlayable(Move move)
        {
            if (Rank(move.FromSquare) == 6 && Rank(move.ToSquare) == 4)
            {
                return this[Down(move.FromSquare, 2)] == Piece.None;
            }
            else
            {
                if (this[Down(move.FromSquare)] != Piece.None) return false;
                if (Rank(move.FromSquare) - Rank(move.ToSquare) != 1) return false;
                return true;
            }
        }

        private bool IsBlackPawnMovePlayable(Move move)
        {
            if (Rank(move.FromSquare) == 1 && Rank(move.ToSquare) == 3)
            {
                return this[Up(move.FromSquare, 2)] == Piece.None;
            }
            else
            {
                if (this[Up(move.FromSquare)] != Piece.None) return false;
                if (Rank(move.ToSquare) - Rank(move.FromSquare) != 1) return false;
                return true;
            }
        }

        public static bool IsSquareAttackedBy(int square, Color color)
        {
            // Work out if specified square is attacked by any piece of specified color

            // First try pawns

            return false;
        }
    }
}
