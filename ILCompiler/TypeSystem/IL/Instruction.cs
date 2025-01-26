namespace ILCompiler.TypeSystem.IL
{
    public class Instruction
    {
        public virtual ILOpcode Opcode { get; }
        public virtual uint Offset { get; }
        public virtual int GetSize() => 1;

        private readonly object? _operand = null;
        public virtual object Operand
        {
            get
            {
                if (_operand is null)
                {
                    throw new InvalidOperationException("Operand is null");
                }
                return _operand;
            }
        }

        public Instruction(ILOpcode opcode, uint offset, object? operand = null)
        {
            Opcode = opcode;
            Offset = offset;
            _operand = operand;
        }
    }
}