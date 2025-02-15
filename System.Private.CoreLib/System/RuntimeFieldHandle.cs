namespace System
{
    public struct RuntimeFieldHandle
    {
        private readonly IntPtr _value;

        private RuntimeFieldHandle(IntPtr value)
        {
            _value = value;
        }

        public IntPtr Value => _value;
    }
}
