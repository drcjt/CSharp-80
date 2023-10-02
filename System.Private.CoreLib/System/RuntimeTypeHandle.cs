namespace System
{
    public readonly struct RuntimeTypeHandle
    {
        private readonly IntPtr _EEType;

        internal RuntimeTypeHandle(IntPtr pEEType)
        {
            _EEType = pEEType;
        }

        internal static unsafe IntPtr GetValueInternal(RuntimeTypeHandle handle) => handle._EEType;

        public readonly bool Equals(RuntimeTypeHandle handle) => handle._EEType == _EEType;
    }
}