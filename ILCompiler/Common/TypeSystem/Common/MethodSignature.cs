using System.Runtime.CompilerServices;
using System.Text;

namespace ILCompiler.Common.TypeSystem.Common
{
    public class MethodParameter
    {
        public TypeDesc Type;
        public readonly string Name;

        public MethodParameter(TypeDesc type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public class MethodSignature : TypeSystemEntity, IEquatable<MethodSignature>
    {
        private readonly MethodParameter[] _parameters;
        public TypeDesc ReturnType { get; init; }

        [IndexerName("Parameter")]
        public MethodParameter this[int index]
        {
            get
            {
                return _parameters[index];
            }
        }

        public int Length => _parameters.Length;

        public MethodParameter[] Parameters => _parameters;
        public MethodSignature(bool isStatic, TypeDesc returnType, MethodParameter[] parameters)
        {
            IsStatic = isStatic;
            ReturnType = returnType;
            _parameters = parameters;
        }

        public bool IsStatic { get; }

        public override TypeSystemContext Context => ReturnType.Context;

        public bool Equals(MethodSignature? other)
        {
            if (this.ReturnType.ToString() != other?.ReturnType.ToString()) return false;
            if (this.Length != other?.Length) return false;

            for (int parameterIndex = 0; parameterIndex < Length; parameterIndex++)
            {
                if (Parameters[parameterIndex].Type.ToString() != other.Parameters[parameterIndex].Type.ToString()) return false;
            }

            return true;
        }

        public bool EquivalentTo(MethodSignature otherSignature)
        {
            if (!TypeIsEqualHelper(ReturnType, otherSignature.ReturnType)) 
                return false;

            if (_parameters.Length != otherSignature._parameters.Length)
                return false;

            // For instance methods ignore first parameter as will be "this"
            var startParameterIndex = IsStatic ? 0 : 1;
            for (int i = startParameterIndex; i < _parameters.Length; i++)
            {
                if (!TypeIsEqualHelper(_parameters[i].Type, otherSignature._parameters[i].Type))
                    return false;
            }

            return true;

            static bool TypeIsEqualHelper(TypeDesc type1, TypeDesc type2)
            {
                if (type1 == type2) 
                    return true;

                if (type1.IsEquivalentTo(type2))
                {
                    return true;
                }

                return false;
            }
        }

        public override string ToString() => ToString(includeReturnType: true);

        public string ToString(bool includeReturnType)
        {
            var sb = new StringBuilder();
            var typeNameFormatter = new TypeNameFormatter();

            if (includeReturnType)
            {
                typeNameFormatter.AppendName(sb, ReturnType);
                sb.Append('(');
            }

            var first = true;
            foreach (var parameter in Parameters)
            {
                if (!first)
                {
                    sb.Append(',');
                }
                first = false;

                typeNameFormatter.AppendName(sb, parameter.Type);
            }

            if (includeReturnType)
            {
                sb.Append(')');
            }
            return sb.ToString();
        }
    }
}
