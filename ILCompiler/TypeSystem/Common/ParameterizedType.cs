using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class ParameterizedType : TypeDesc
    {
        public TypeDesc ParameterType { get; init; }
        public ParameterizedType(TypeDesc parameterType)
        {
            ParameterType = parameterType;
        }

        public override TypeSystemContext Context => ParameterType.Context;

        public override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            return ParameterType.IsCanonicalSubtype(policy);
        }

        public override bool IsRuntimeDeterminedSubtype => ParameterType.IsRuntimeDeterminedSubtype;
    }
}
