namespace System.Collections
{
    public interface ICollection : IEnumerable
    {
        void CopyTo(Array array, int index);

        int Count { get; }
    }
}
