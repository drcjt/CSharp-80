using System.Collections;
using System.Text;

namespace ILCompiler.Compiler.Ssa
{
    public sealed class VariableSet : IEnumerable<int>, IEquatable<VariableSet>
    {
        private readonly ISet<int> _variables = new HashSet<int>();

        public static VariableSet Empty => new();

        public int Count => _variables.Count;
        public bool IsMember(int localNumber) => _variables.Contains(localNumber);
        public void Clear() => _variables.Clear();
        public void AddElem(int localNumber) => _variables.Add(localNumber);
        public void Union(VariableSet other) => _variables.UnionWith(other._variables);
        public bool Equals(VariableSet? other)
        {
            if (other == null) return false;
            return _variables.SetEquals(other._variables);
        }
        
        public static VariableSet Union(VariableSet left, VariableSet right)
        {
            var result = new VariableSet ();
            result.Union(left);
            result.Union(right);
            return result;
        }

        public void Assign(VariableSet varSet)
        {
            _variables.Clear();
            _variables.UnionWith(varSet._variables);
        }

        public string DisplayVarSet(VariableSet allVars)
        {
            var sb = new StringBuilder();
            sb.Append('{');

            var needSpace = false;

            foreach (var v in allVars)
            {
                if (IsMember(v))
                {
                    if (needSpace)
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        needSpace = true;
                    }
                    sb.AppendFormat("V{0:00}", v);
                }
                else
                {
                    if (needSpace)
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        needSpace = true;
                    }

                    sb.Append("   ");
                }
            }
            sb.Append('}');

            return sb.ToString();
        }

        public IEnumerator<int> GetEnumerator() => _variables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _variables.GetEnumerator();
    }
}
