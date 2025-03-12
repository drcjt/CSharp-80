using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class MetadataType : DefType 
    {
        public abstract ClassLayoutMetadata GetClassLayout();

        public abstract bool IsSequentialLayout { get; }

        public abstract DefType[] ExplicitlyImplementedInterfaces
        {
            get;
        }

        public abstract MetadataType? MetadataBaseType { get; }

        public abstract MethodImplRecord[] FindMethodsImplWithMatchingDeclName(string name);

        public override bool IsCanonicalSubtype(CanonicalFormKind policy) => false;

        public abstract override DefType? ContainingType { get; }
    }

    public struct ClassLayoutMetadata
    {
        public int PackingSize;
        public int Size;
        public FieldAndOffset[] Offsets;
    }
}
