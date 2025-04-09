namespace ILCompiler.TypeSystem.IL
{
    public class Instruction
    {
        public virtual ILOpcode Opcode { get; }
        public virtual uint Offset { get; set; }
        public virtual int GetSize() => 1;

        private object? _operand = null;
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
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value), "Operand cannot be null");
                }
                _operand = value;
            }
        }

        public Instruction(ILOpcode opcode, uint offset, object? operand = null)
        {
            Opcode = opcode;
            Offset = offset;
            _operand = operand;
        }

        public Instruction(ILOpcode opcode, object? operand = null)
        {
            Opcode = opcode;
            _operand = operand;
        }
    }
}