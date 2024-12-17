namespace System.Collections
{
    public interface IEnumerator
    {
        bool MoveNext();

#nullable disable
        object Current
#nullable restore
        {
            get;
        }

        void Reset();
    }
}
