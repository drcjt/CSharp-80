namespace Internal.Runtime
{
    internal unsafe struct EEType
    {
        private ushort _usFlags;
        private ushort _usBaseSize;
        private EEType* _relatedType;

        public ushort GetFlags() { return _usFlags; }

        internal EEType* RelatedType => _relatedType;
    }
}
