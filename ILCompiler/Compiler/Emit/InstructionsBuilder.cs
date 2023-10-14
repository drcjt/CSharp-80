﻿using System.Text;

namespace ILCompiler.Compiler.Emit
{
    public class InstructionsBuilder
    {
        public IList<Instruction> Instructions { get; } = new List<Instruction>();

        public void AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
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
        public void Adc(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Adc, target, source));
        public void Add(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Add, target, source));
        public void Add(Register8 target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Add, target, source));
        public void And(Register8 target) => AddInstruction(Instruction.Create(Opcode.And, target));
        public void Call(string target) => AddInstruction(Instruction.CreateBranch(Opcode.Call, target));
        public void Call(ushort target) => AddInstruction(Instruction.CreateBranch(Opcode.Call, target));
        public void Dec(Register16 register) => AddInstruction(Instruction.Create(Opcode.Dec, register));
        public void Dec(Register8 register) => AddInstruction(Instruction.Create(Opcode.Dec, register));
        public void Exx() => AddInstruction(Instruction.Create(Opcode.Exx));
        public void Halt() => AddInstruction(Instruction.Create(Opcode.Halt));
        public void Inc(Register16 register) => AddInstruction(Instruction.Create(Opcode.Inc, register));
        public void Jp(string target) => AddInstruction(Instruction.CreateBranch(Opcode.Jp, target));
        public void Jp(MemoryOperand target)
        {
            if (target.Register != Register.HL)
            {
                throw new NotSupportedException("Indirect jump only supports HL");
            }
            AddInstruction(Instruction.Create(Opcode.Jp, target));
        }
        public void Jp(Condition condition, string target) => AddInstruction(Instruction.CreateBranch(Opcode.Jp, condition, target));
        public void Ld(Register8 target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register8 target, ushort source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, byte source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(MemoryOperand target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register8 target, MemoryOperand source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, MemoryOperand source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, string source) => AddInstruction(Instruction.Create(Opcode.Ld, target, source));
        public void Ld(Register16 target, ushort immediate) => AddInstruction(Instruction.Create(Opcode.Ld, target, immediate));
        public void Ld(Register16 target, short immediate) => Ld(target, (ushort)immediate);
        public void Or(Register8 target) => AddInstruction(Instruction.Create(Opcode.Or, target));
        public void Pop(Register16 register) => AddInstruction(Instruction.Create(Opcode.Pop, register));
        public void Push(Register16 register) => AddInstruction(Instruction.Create(Opcode.Push, register));
        public void Ret() => AddInstruction(Instruction.Create(Opcode.Ret));
        public void Rr(Register8 target) => AddInstruction(Instruction.Create(Opcode.Rr, target));
        public void Rst(ushort target) => AddInstruction(Instruction.Create(Opcode.Rst, target));
        public void Sbc(Register16 target, Register16 source) => AddInstruction(Instruction.Create(Opcode.Sbc, target, source));
        public void Sbc(Register8 target, Register8 source) => AddInstruction(Instruction.Create(Opcode.Sbc, target, source));
        public void Srl(Register8 target) => AddInstruction(Instruction.Create(Opcode.Srl, target));

        // Pseudo instructions
        public void Db(string source) => AddInstruction(Instruction.CreateDeclareByte(Opcode.Db, source));
        public void Db(string label, string source) => AddInstruction(Instruction.CreateDeclareByte(Opcode.Db, source, label));
        public void Db(byte b) => AddInstruction(Instruction.Create(Opcode.Db, b));
        public void Dw(string source) => AddInstruction(Instruction.CreateDeclareWord(Opcode.Dw, source));
        public void Dw(ushort w) => AddInstruction(Instruction.Create(Opcode.Dw, w));
        public void Defs(ushort size) => AddInstruction(Instruction.Create(Opcode.Defs, size));
        public void Dc(ushort count, ushort value) => AddInstruction(Instruction.Create(Opcode.Dc, count, value));
        public void Org(ushort target) => AddInstruction(Instruction.Create(Opcode.Org, target));
        public void End(string label) => AddInstruction(Instruction.Create(Opcode.End, label));
    }
}