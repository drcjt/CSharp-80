using System.Collections;

namespace ILCompiler.Compiler
{
    public class LocalVariableTable : IEnumerable<LocalVariableDescriptor>
    {
        private readonly IList<LocalVariableDescriptor> _locals;

        public LocalVariableDescriptor this[int i] => _locals[i];

        public void Add(LocalVariableDescriptor local) => _locals.Add(local);
        public void Insert(int index, LocalVariableDescriptor local) => _locals.Insert(index, local);

        public int Count => _locals.Count;

        public IEnumerator<LocalVariableDescriptor> GetEnumerator() => _locals.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_locals).GetEnumerator();

        public LocalVariableTable()
        {
            _locals = new List<LocalVariableDescriptor>();
        }

        public int GrabTemp(VarType type, int? exactSize)
        {
            var temp = new LocalVariableDescriptor()
            {
                IsParameter = false,
                IsTemp = true,
                ExactSize = exactSize ?? 0,
                Type = type
            };

            _locals.Add(temp);

            return _locals.Count - 1;
        }

        public void ResetCount(int count)
        {
            // Remove any variables added after count
            int removeAt = count;
            int removeCount = _locals.Count - count;

            while (removeCount-- > 0)
            {
                _locals.RemoveAt(removeAt);
            }
        }
    }
}
