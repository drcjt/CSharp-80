namespace ILCompiler.z80
{
    public class LabelInstruction : Z80Instruction
    {
        public LabelInstruction(string label) : base(label, null, string.Empty)
        {
        }
    }
}
