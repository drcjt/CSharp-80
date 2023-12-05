namespace System.Collections
{
    public interface IEnumerator
    {
        bool MoveNext();

        object Current
        {
            get;
        }

        void Reset();
    }
}
