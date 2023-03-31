namespace ILCompiler.Compiler.Emit
{
    public partial class Emitter
    {
        public IList<Instruction> Instructions { get; }

        public Emitter()
        {
            Instructions = new List<Instruction>();
        }

        public void Reset()
        {
            Instructions.Clear();
        }

        public void AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
