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

        public void EmitInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
