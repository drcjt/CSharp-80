namespace System
{
    public interface IEquatable<T> // where T : allows ref struct
    {
        bool Equals(T? other);
    }
}
