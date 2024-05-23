namespace ILCompiler.Common.TypeSystem.Common
{
    public class Instantiation
    {
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
    }
}
