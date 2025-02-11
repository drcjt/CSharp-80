namespace System.Collections.Tests
{
    public class Equatable : IEquatable<Equatable>
    {
        public Equatable(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public bool Equals(Equatable? other)
        {
            return other != null && Value == other.Value;
        }

        public override int GetHashCode() => Value;
    }
}
