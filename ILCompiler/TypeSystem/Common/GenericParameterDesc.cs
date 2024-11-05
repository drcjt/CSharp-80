using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public enum GenericParameterKind
    {
        Type,
        Method,
    }

    public abstract class GenericParameterDesc : TypeDesc
    {
        public abstract int Index { get; }

        public override string Name => string.Concat("T", Index.ToString());

        public abstract GenericParameterKind Kind { get; }
        public abstract TypeSystemEntity AssociatedTypeOrMethod { get; }

        public override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            throw new Exception("IsCanonicalSubType of an indefinite type");
        }
        public override bool IsRuntimeDeterminedSubtype => throw new Exception("IsRuntimeDeterminedType of an indefinite type");
    }
}