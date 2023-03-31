namespace ILCompiler.Compiler.Emit
{
    public partial class Emitter
    {
        public IList<Instruction> Instructions { get; }
        ulong _currentLabelId;

        public Emitter()
        {
            Instructions = new List<Instruction>();
        }

        public void Reset()
        {
            Instructions.Clear();
        }

        public Label CreateLabel(string name)
        {
            _currentLabelId++;
            var label = new Label(name, _currentLabelId);
            return label;
        }

        public void AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
