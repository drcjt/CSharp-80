namespace Z80Assembler
{
    public class LabelInstruction : Instruction
    {
        public LabelInstruction(string label) : base(label, null, string.Empty)
        {
        }
    }
}
