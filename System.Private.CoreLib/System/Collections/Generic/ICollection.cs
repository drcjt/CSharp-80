namespace System.Collections.Generic
{
    public interface ICollection<T> : IEnumerable<T>
    {
        void CopyTo(T[] array, int index);

        int Count { get; }
        void Add(T item);
        bool Remove(T item);
        void Clear();
        bool Contains(T item);
    }
}
