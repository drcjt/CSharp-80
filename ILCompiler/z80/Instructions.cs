namespace ILCompiler.z80
{
    public partial class Assembly
    {
        public void RET()
        {
            _instructions.Add(new Instruction(string.Empty, "RET", string.Empty, string.Empty));
        }

        public void POP(R16Type target)
        {
            _instructions.Add(new Instruction(string.Empty, "POP", target.ToString(), string.Empty));
        }

        public void PUSH(R16Type target)
        {
            _instructions.Add(new Instruction(string.Empty, "PUSH", target.ToString(), string.Empty));
        }

        public void LD(R16Type target, sbyte source)
        {
            _instructions.Add(new Instruction(string.Empty, "LD", target.ToString() + ", " + string.Format("{0:X}H", source), string.Empty));
        }

        public void CALL(string label)
        {
            _instructions.Add(new Instruction(string.Empty, "CALL", label, string.Empty));
        }
    }
}
