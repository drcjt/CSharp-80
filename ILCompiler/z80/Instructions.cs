using System;

namespace ILCompiler.z80
{
    public partial class Assembly
    {
        public void RET()
        {
            _instructions.Add(new Instruction(string.Empty, "RET", string.Empty));
        }

        public void POP(R16Type target)
        {
            var lastInstruction = _instructions[^1];
            if (lastInstruction.Opcode == "PUSH" && lastInstruction.Operands == target.ToString())
            {
                // Eliminate PUSH XX followed by POP XX
                _instructions.RemoveAt(_instructions.Count-1);
            }
            else
            {
                _instructions.Add(new Instruction(string.Empty, "POP", target.ToString()));
            }
        }

        public void PUSH(R16Type target)
        {
            _instructions.Add(new Instruction(string.Empty, "PUSH", target.ToString()));
        }

        public void LD(R16Type target, sbyte source)
        {
            _instructions.Add(new Instruction(string.Empty, "LD", target.ToString() + ", " + string.Format("{0:X}H", source)));
        }

        public void LD(R8Type target, R8Type source)
        {
            _instructions.Add(new Instruction(string.Empty, "LD", target.ToString() + ", " + source.ToString()));
        }

        public void CALL(string label)
        {
            _instructions.Add(new Instruction(string.Empty, "CALL", label));
        }

        public void CALL(UInt16 target)
        {
            _instructions.Add(new Instruction(string.Empty, "CALL", string.Format("{0:X}H", target)));
        }
    }
}
