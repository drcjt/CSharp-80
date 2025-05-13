namespace System
{
    public readonly struct RuntimeTypeHandle : IEquatable<RuntimeTypeHandle>
    {
        private readonly IntPtr _EEType;

        internal RuntimeTypeHandle(IntPtr pEEType)
        {
            _EEType = pEEType;
        }

        internal static unsafe IntPtr GetValueInternal(RuntimeTypeHandle handle) => handle._EEType;

        public override bool Equals(object? obj)
        {
            if (obj is not RuntimeTypeHandle)
                return false;

            return _EEType == ((RuntimeTypeHandle)obj)._EEType;
        }

        public bool Equals(RuntimeTypeHandle handle) => _EEType == handle._EEType;

        public override int GetHashCode() => _EEType.GetHashCode();

        public override string ToString() => _EEType.ToString();
    }
}