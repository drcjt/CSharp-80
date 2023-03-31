namespace ILCompiler.Compiler.Emit
{
    public class LabelInstruction : Instruction
    {
        public LabelInstruction(string label) : base(label, opcode: null, operands: string.Empty)
        {
        }
    }
}
