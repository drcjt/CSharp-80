using System.Text;

namespace ILCompiler.Compiler.Emit
{
    public class Emitter
    {
        public IList<Instruction> Instructions { get; } = new List<Instruction>();

        public void EmitInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
        }

        public void Reset()
        {
            Instructions.Clear();
        }

        public void CreateLabel(string name)
        {
            EmitInstruction(Instruction.Create(name));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var instruction in Instructions)
            {
                stringBuilder.AppendLine(instruction.ToString());
            }
            return stringBuilder.ToString();
        }

        public void Adc(Register16 target, Register16 source) => EmitInstruction(Instruction.Create(Opcode.Adc, target, source));
        public void Add(Register16 target, Register16 source) => EmitInstruction(Instruction.Create(Opcode.Add, target, source));
        public void Add(Register8 target, Register8 source) => EmitInstruction(Instruction.Create(Opcode.Add, target, source));
        public void And(Register8 target) => EmitInstruction(Instruction.Create(Opcode.And, target));
        public void Call(string target) => EmitInstruction(Instruction.CreateBranch(Opcode.Call, target));
        public void Call(ushort target) => EmitInstruction(Instruction.CreateBranch(Opcode.Call, target));
        public void Dec(Register16 register) => EmitInstruction(Instruction.Create(Opcode.Dec, register));
        public void Dec(Register8 register) => EmitInstruction(Instruction.Create(Opcode.Dec, register));
        public void Exx() => EmitInstruction(Instruction.Create(Opcode.Exx));
        public void Halt() => EmitInstruction(Instruction.Create(Opcode.Halt));
        public void Inc(Register16 register) => EmitInstruction(Instruction.Create(Opcode.Inc, register));
        public void Jp(string target) => EmitInstruction(Instruction.CreateBranch(Opcode.Jp, target));
        public void Jp(Condition condition, string target) => EmitInstruction(Instruction.CreateBranch(Opcode.Jp, condition, target));
        public void Ld(Register8 target, Register8 source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, Register16 source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register8 target, ushort source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, Register8 source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, byte source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, Register16 source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register8 target, MemoryOperand source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, MemoryOperand source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, string source) => EmitInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, ushort immediate) => EmitInstruction(Instruction.Create(Opcode.Ld, target, immediate));
        public void Ld(Register16 target, short immediate) => Ld(target, (ushort)immediate);
        public void Or(Register8 target) => EmitInstruction(Instruction.Create(Opcode.Or, target));
        public void Pop(Register16 register) => EmitInstruction(Instruction.Create(Opcode.Pop, register));
        public void Push(Register16 register) => EmitInstruction(Instruction.Create(Opcode.Push, register));
        public void Ret() => EmitInstruction(Instruction.Create(Opcode.Ret));
        public void Rr(Register8 target) => EmitInstruction(Instruction.Create(Opcode.Rr, target));
        public void Rst(ushort target) => EmitInstruction(Instruction.Create(Opcode.Call, target));
        public void Sbc(Register16 target, Register16 source) => EmitInstruction(Instruction.Create(Opcode.Sbc, target, source));
        public void Sbc(Register8 target, Register8 source) => EmitInstruction(Instruction.Create(Opcode.Sbc, target, source));
        public void Srl(Register8 target) => EmitInstruction(Instruction.Create(Opcode.Srl, target));

        // Pseudo instructions
        public void Db(string source) => EmitInstruction(Instruction.CreateDeclareByte(Opcode.Db, source));
        public void Db(string label, string source) => EmitInstruction(Instruction.CreateDeclareByte(Opcode.Db, source, label));
        public void Db(byte b) => EmitInstruction(Instruction.Create(Opcode.Db, b));
        public void Defs(ushort size) => EmitInstruction(Instruction.Create(Opcode.Defs, size));
        public void Dc(ushort count, ushort value) => EmitInstruction(Instruction.Create(Opcode.Dc, count, value));
        public void Org(ushort target) => EmitInstruction(Instruction.Create(Opcode.Org, target));
        public void End(string label) => EmitInstruction(Instruction.Create(Opcode.End, label));
    }
}
