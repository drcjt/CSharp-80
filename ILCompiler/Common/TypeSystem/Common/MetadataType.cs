namespace ILCompiler.Common.TypeSystem.Common
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
    }

    public struct ClassLayoutMetadata
    {
        public int PackingSize;
        public int Size;
        public FieldAndOffset[] Offsets;
    }
}
