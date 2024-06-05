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

    }
}