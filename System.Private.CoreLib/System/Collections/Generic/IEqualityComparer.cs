namespace System.Collections.Generic
{
    public interface IEqualityComparer<in T> //where T : allows ref struct
    {
        bool Equals(T? x, T? y);
        int GetHashCode(T obj);
    }
}
