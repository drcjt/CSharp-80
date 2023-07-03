namespace Internal.Runtime
{
    internal unsafe struct EEType
    {
        private ushort _usBaseSize;
        private EEType* _relatedType;

        internal EEType* RelatedType => _relatedType;
    }
}
