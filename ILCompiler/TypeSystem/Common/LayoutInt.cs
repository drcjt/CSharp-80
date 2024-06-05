using System.Diagnostics;

namespace ILCompiler.TypeSystem.Common
{
    public class LayoutInt
    {
        public int Value { get; private set; }

        public static readonly LayoutInt Zero = new(0);
        public static readonly LayoutInt One = new(1);

        public LayoutInt(int value)
        {
            Value = value;
        }

        public LayoutInt(uint value) : this((int)value)
        {
        }

        public int AsInt => Value;

        public static bool operator ==(LayoutInt left, LayoutInt right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(LayoutInt left, LayoutInt right)
        {
            return left.Value != right.Value;
        }

        public static LayoutInt operator +(LayoutInt left, LayoutInt right)
        {
            return new LayoutInt(checked(left.Value + right.Value));
        }

        public override bool Equals(object? obj)
        {
            if (obj is LayoutInt layoutInt)
            {
                return this == layoutInt;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static LayoutInt Min(LayoutInt left, LayoutInt right)
        {
            return new LayoutInt(Math.Min(left.Value, right.Value));
        }

        public static LayoutInt Max(LayoutInt left, LayoutInt right)
        {
            return new LayoutInt(Math.Max(left.Value, right.Value));
        }

        public static LayoutInt AlignUp(LayoutInt value, LayoutInt alignment, TargetDetails target)
        {
            Debug.Assert(alignment.Value <= target.MaximumAlignment);
            Debug.Assert(alignment.Value >= 1 || ((value.Value == 0) && (alignment.Value == 0)));

            return new LayoutInt(AlignmentHelper.AlignUp(value.Value, alignment.Value));
        }
    }
}