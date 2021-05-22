namespace ILCompiler.z80
{
    public class LabelInstruction : Instruction
    {
        public LabelInstruction(string label) : base(label, null, string.Empty)
        {
        }
    }
}
