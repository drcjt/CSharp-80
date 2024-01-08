namespace System.Runtime
{
    internal struct StackFrameIterator
    {
        internal ushort StackPointer;
        internal ushort FramePointer;
        internal ushort InstructionPointer;
    }
}
