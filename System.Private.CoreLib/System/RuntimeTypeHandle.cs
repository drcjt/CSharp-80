namespace System
{
    public struct RuntimeTypeHandle
    {
        private IntPtr _EEType;

        internal RuntimeTypeHandle(IntPtr pEEType)
        {
            _EEType = pEEType;
        }

        internal static unsafe IntPtr GetValueInternal(RuntimeTypeHandle handle)
        {
            return handle._EEType;
        }

        public bool Equals(RuntimeTypeHandle handle)
        {
            if (_EEType == handle._EEType)
                return true;
            return false;
        }
    }
}