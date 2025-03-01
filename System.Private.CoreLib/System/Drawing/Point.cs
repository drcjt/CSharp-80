using System.Diagnostics.CodeAnalysis;

namespace System.Drawing
{
    public struct Point : IEquatable<Point>
    {
        private int x;
        private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { readonly get => x; set => x = value; }
        public int Y { readonly get => y; set => y = value; }

        public static bool operator ==(Point left, Point right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(Point left, Point right) => !(left == right);

        public bool Equals(Point other) => this == other;

        public override bool Equals([NotNullWhen(true)]object? obj) => obj is Point && Equals((Point)obj);

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
