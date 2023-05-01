namespace ILCompiler.Compiler.Emit
{
    public class MemoryOperandFactory
    {
        public MemoryOperand this[MemoryOperand operand] => new(operand.Register, operand.Displacement);

        public MemoryOperand this[Register16 register] => new(register, 0);

        public MemoryOperand this[short offset] => new(Register.None, offset);

        public MemoryOperand this[string label] => new(Register.None, 0, label);
    }
}
