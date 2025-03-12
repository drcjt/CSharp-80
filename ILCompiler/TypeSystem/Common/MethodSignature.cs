using System.Runtime.CompilerServices;
using System.Text;

namespace ILCompiler.TypeSystem.Common
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

    [Flags]
    public enum MethodSignatureFlags
    {
        None =  0x0000,
        Static = 0x0010,
        ExplicitThis = 0x0020,
    }

    public sealed class MethodSignature : TypeSystemEntity, IEquatable<MethodSignature>
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

        private readonly MethodSignatureFlags _flags;

        public MethodSignature(MethodSignatureFlags flags, TypeDesc returnType, MethodParameter[] parameters)
        {
            _flags = flags;
            ReturnType = returnType;
            _parameters = parameters;
        }

        public MethodSignatureFlags Flags => _flags;
        public bool IsStatic => (_flags & MethodSignatureFlags.Static) != 0;
        public bool ExplicitThis => (_flags & MethodSignatureFlags.ExplicitThis) != 0;

        public override TypeSystemContext Context => ReturnType.Context;

        private bool Equals(MethodSignature? other, bool allowEquivalence)
        {
            if (!TypeIsEqualHelper(this.ReturnType, other!.ReturnType, allowEquivalence))
                return false;

            if (this.Length != other.Length) return false;


            for (int parameterIndex = 0; parameterIndex < Length; parameterIndex++)
            {
                if (!TypeIsEqualHelper(Parameters[parameterIndex].Type, other.Parameters[parameterIndex].Type, allowEquivalence)) return false;
            }
            return true;

            static bool TypeIsEqualHelper(TypeDesc type1, TypeDesc type2, bool allowEquivalence)
            {
                if (type1 == type2)
                    return true;

                if (allowEquivalence && type1.IsEquivalentTo(type2))
                {
                    return true;
                }
                return false;
            }
        }

        public bool Equals(MethodSignature? other) => Equals(other, allowEquivalence: false);

        public override bool Equals(object? obj) => obj is MethodSignature signature && Equals(signature);

        public bool EquivalentTo(MethodSignature otherSignature)
        {
            return Equals(otherSignature, allowEquivalence: true);
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

        public MethodSignature ApplySubstitution(Instantiation? substitution)
        {
            if (substitution is null)
                return this;

            MethodParameter[] newParameters = new MethodParameter[Length];
            var returnTypeNew = ReturnType.InstantiateSignature(substitution, default(Instantiation));

            int parameterIndex = 0;
            foreach (var parameter in Parameters)
            {
                var newParameter = parameter.Type.InstantiateSignature(substitution, default(Instantiation));
                newParameters[parameterIndex++] = new MethodParameter(newParameter, parameter.Name);
            }

            return new MethodSignature(Flags, returnTypeNew, newParameters);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
