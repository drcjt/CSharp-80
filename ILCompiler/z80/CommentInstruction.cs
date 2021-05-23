namespace ILCompiler.z80
{
    public class CommentInstruction : Z80Instruction
    {
        public CommentInstruction(string comment) : base(string.Empty, null, string.Empty, comment)
        {
        }
    }
}
