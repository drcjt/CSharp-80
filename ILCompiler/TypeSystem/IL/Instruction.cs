namespace ILCompiler.TypeSystem.IL
{
    public abstract class Instruction
    {
        public abstract ILOpcode Opcode { get; }
        public abstract uint Offset { get; }
        public abstract int GetSize();
        public abstract bool OperandIsNotNull { get; }
        public abstract object GetOperand();
    }
}