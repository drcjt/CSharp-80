using System.Text;

namespace ILCompiler.Compiler.Emit
{
    public class InstructionsBuilder
    {
        public IList<Instruction> Instructions { get; } = new List<Instruction>();

        public void AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
        }

        public int ReserveByte()
        {
            var reservationIndex = Instructions.Count;

            AddInstruction(Instruction.Create(Opcode.Db, (ushort)0));

            return reservationIndex;
        }

        public void UpdateReservation(int reservationIndex, Instruction instruction)
        {
            Instructions[reservationIndex] = instruction;
        }

        public void Reset()
        {
            Instructions.Clear();
        }

        public void Label(string name)
        {
            AddInstruction(Instruction.Create(name));
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

        public void Comment(string comment) => AddInstruction(Instruction.CreateComment(comment));
        public void Adc(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Adc, target, source, 2));
        public void Add(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Add, target, source, target.IsIndexRegister() ? 2 : 1));
        public void Add(Register8 target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Add, target, source));
        public void And(Register8 target) => AddInstruction(Instruction.Create(Opcode.And, target));
        public void Call(string target) => AddInstruction(Instruction.CreateBranch(Opcode.Call, target));
        public void Call(ushort target) => AddInstruction(Instruction.CreateBranch(Opcode.Call, target));
        public void Call(Condition condition, string target) => AddInstruction(Instruction.CreateBranch(Opcode.Call, condition, target));
        public void Dec(Register16 register) => AddInstruction(Instruction.Create(Opcode.Dec, register, register.IsIndexRegister() ? 2 : 1));
        public void Dec(Register8 register) => AddInstruction(Instruction.Create(Opcode.Dec, register));

        public void Exx() => AddInstruction(Instruction.Create(Opcode.Exx));
        public void Ex(Register16 register1, Register16 register2)
        {
            if (register1 != Register.DE && register2 != Register.HL)
            {
                throw new NotSupportedException("Can only exchange DE and HL");
            }
            AddInstruction(Instruction.Create(Opcode.Ex, register1, register2));
        }
        public void Halt() => AddInstruction(Instruction.Create(Opcode.Halt));
        public void Inc(Register16 register) => AddInstruction(Instruction.Create(Opcode.Inc, register, register.IsIndexRegister() ? 2 : 1));
        public void Inc(Register8 register) => AddInstruction(Instruction.Create(Opcode.Inc, register));
        public void Jp(string target) => AddInstruction(Instruction.CreateBranch(Opcode.Jp, target));
        public void Jp(MemoryOperand target)
        {
            if (target.Register != Register.HL)
            {
                throw new NotSupportedException("Indirect jump only supports HL");
            }
            AddInstruction(Instruction.Create(Opcode.Jp, target, 1));
        }
        public void Jp(Condition condition, string target) => AddInstruction(Instruction.CreateBranch(Opcode.Jp, condition, target));
        public void Jr(string target) => AddInstruction(Instruction.CreateBranch(Opcode.Jr, target, 2));
        public void Jr(Condition condition, string target) => AddInstruction(Instruction.CreateBranch(Opcode.Jr, condition, target, 2));
        public void Ld(Register8 target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, source.IsIndexRegister() ? 2 : 1));
        public void Ld(Register8 target, ushort source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, 2));
        public void Ld(MemoryOperand target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, target.Register.IsIndexRegister() ? 3 : 1));
        public void Ld(MemoryOperand target, byte source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, target.Register == Register.HL ? 3 : 4));
        public void Ld(Register8 target, MemoryOperand source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, source.Register.IsIndexRegister() ? 3 : 1));
        public void Ld(Register16 target, MemoryOperand source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, target == Register.HL ? 3 : 4));
        public void Ld(Register16 target, string source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source, target.IsIndexRegister() ? 4 : 3));
        public void Ld(Register16 target, ushort immediate) => AddInstruction(Instruction.Create(Opcode.Ld, target, immediate, target.IsIndexRegister() ? 4 : 3));
        public void Ld(Register16 target, short immediate) => Ld(target, (ushort)immediate);
        public void Or(Register8 target) => AddInstruction(Instruction.Create(Opcode.Or, target));
        public void Pop(Register16 register) => AddInstruction(Instruction.Create(Opcode.Pop, register, register.IsIndexRegister() ? 2 : 1));
        public void Push(Register16 register) => AddInstruction(Instruction.Create(Opcode.Push, register, register.IsIndexRegister() ? 2 : 1));
        public void Ret() => AddInstruction(Instruction.Create(Opcode.Ret));

        public void Rla() => AddInstruction(Instruction.Create(Opcode.Rla));
        public void Rl(Register8 target) => AddInstruction(Instruction.Create(Opcode.Rl, target, 2));
        public void Rra() => AddInstruction(Instruction.Create(Opcode.Rra));
        public void Sra(Register8 target) => AddInstruction(Instruction.Create(Opcode.Sra, target, 2));
        public void Rr(Register8 target) => AddInstruction(Instruction.Create(Opcode.Rr, target, 2));

        public void Rst(ushort target) => AddInstruction(Instruction.Create(Opcode.Rst, target));
        public void Sbc(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Sbc, target, source, 2));
        public void Sbc(Register8 target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Sbc, target, source, 2));
        public void Srl(Register8 target) => AddInstruction(Instruction.Create(Opcode.Srl, target, 2));

        // Pseudo instructions
        public void Db(string source) => AddInstruction(Instruction.CreateDeclareByte(Opcode.Db, source));
        public void Db(string label, string source) => AddInstruction(Instruction.CreateDeclareByte(Opcode.Db, source, label));
        public void Db(byte b, string? comment = null) => AddInstruction(Instruction.Create(Opcode.Db, b, comment));
        public void Dw(string source, string? comment = null) => AddInstruction(Instruction.CreateDeclareWord(Opcode.Dw, source, comment));
        public void Dw(ushort w, string? comment = null) => AddInstruction(Instruction.Create(Opcode.Dw, w, comment, 2));
        public void Defs(ushort size) => AddInstruction(Instruction.Create(Opcode.Defs, size, null, size));
        public void Dc(ushort count, ushort value) => AddInstruction(Instruction.Create(Opcode.Dc, count, value));
        public void Org(ushort target) => AddInstruction(Instruction.Create(Opcode.Org, target, null, 0));
        public void End(string label) => AddInstruction(Instruction.Create(Opcode.End, label));
        public void Equ(string label, ushort value) => AddInstruction(Instruction.Create(Opcode.Equ, label, value ));
    }
}
