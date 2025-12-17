namespace ILCompiler.Compiler.ValueNumbering
{
    public class VNDefFuncApp(VNFunc func, ValueNumber[] args)
    {
        private readonly VNFunc _func = func;
        private readonly ValueNumber[] _args = args;

        public override bool Equals(object? obj)
        {
            if (obj is VNDefFuncApp other)
            {
                bool result = (_func == other._func);
                for (int i = 0; i < _args.Length; i++)
                {
                    result = result && _args[i] == other._args[i];
                }
                return result;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = _func.GetHashCode();
            foreach (ValueNumber arg in _args)
            {
                hash = (hash * 397) ^ arg.GetHashCode();
            }
            return hash;
        }
    }
}
