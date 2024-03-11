namespace ILCompiler.Common.TypeSystem.Common
{
    public struct ClassLayoutMetadata
    {
        public int PackingSize;
        public int Size;
        public FieldAndOffset[] Offsets;
    }
}
