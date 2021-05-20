namespace ILCompiler.z80
{
    public class LabelInstruction : Instruction
    {
        public LabelInstruction(string label) : base(label, string.Empty, string.Empty, string.Empty)
        {
        }
    }
}
