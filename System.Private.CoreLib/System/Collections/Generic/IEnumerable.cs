namespace System.Collections.Generic
{
    public interface IEnumerable<out T> : IEnumerable
        // where T : allows ref struct
    {
        new IEnumerator<T> GetEnumerator();
    }
}
