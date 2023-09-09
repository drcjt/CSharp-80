namespace ILCompiler.Compiler.Ssa
{
    // A resizable array of Ssa Definitions
    public class SsaDefList<T>
    {
        IList<T> _list = new List<T>();

        static int MinSsaNumber { get; } = 1;

        public int AllocSsaNumber(Func<T> ctor)
        {
            var ssaNumber = SsaDefList<T>.MinSsaNumber + _list.Count;
            _list.Add(ctor());

            return ssaNumber;
        }

        public int Count { get { return _list.Count; } }

        public T SsaDefinitionByIndex(int index) => _list[index];

        public bool IsValidSsaNumber(int ssaNumber) => MinSsaNumber <= ssaNumber && (ssaNumber < (MinSsaNumber + _list.Count));

        public T SsaDefinition(int ssaNumber) => SsaDefinitionByIndex(ssaNumber - MinSsaNumber);

        public int GetSsaNumber(T ssaDefinition)
        {
            var index = _list.IndexOf(ssaDefinition);
            if (index == - 1) throw new ArgumentException($"Invalid ssaDefinition {ssaDefinition}.", nameof(ssaDefinition));
            return index + MinSsaNumber;
        }
    }
}
