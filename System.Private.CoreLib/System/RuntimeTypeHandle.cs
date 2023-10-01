namespace System
{
    public struct RuntimeTypeHandle
    {
        private EETypePtr _EEType;

        internal static unsafe IntPtr GetValueInternal(RuntimeTypeHandle handle)
        {
            return (IntPtr)handle._EEType.ToPointer();
        }
    }
}
