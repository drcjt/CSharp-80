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

        public bool Equals(RuntimeTypeHandle handle) => _EEType == handle._EEType;

        public override string ToString() => _EEType.ToString();
    }
}