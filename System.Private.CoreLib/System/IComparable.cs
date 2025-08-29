namespace System
{
    public interface IComparable
    {
        int CompareTo(object? obj);
    }

    public interface IComparable<in T> /* where T : allows ref struct */
    {
        int CompareTo(T? other);
    }
}
