using System.Text;

namespace ILCompiler.TypeSystem.Common
{
    public class Instantiation
    {
        public static readonly Instantiation Empty = new Instantiation(TypeDesc.EmptyTypes);

        private readonly TypeDesc[] _genericParameters;

        public Instantiation(params TypeDesc[] genericParameters)
        {
            _genericParameters = genericParameters;
        }

        public TypeDesc this[int index]
        {
            get { return _genericParameters[index]; }
        }

        public int Length => _genericParameters.Length;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var t in _genericParameters)
            {
                sb.Append(t.ToString());
                sb.Append(',');
            }
            return sb.ToString();
          }
    }
}