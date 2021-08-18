using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Assembler
{
    public partial class Assembler
    {
        public void Ret()
        {
            AddInstruction(new Instruction(Opcode.Ret));
        }

        public void Org(short address)
        {
            AddInstruction(new Instruction(Opcode.Org, string.Format("{0:X}H", address)));
        }

        public void Call(string label)
        {
            AddInstruction(new Instruction(Opcode.Call, label));
        }

        public void Call(UInt16 target)
        {
            AddInstruction(new Instruction(Opcode.Call, string.Format("{0:X}H", target)));
        }

        public void Pop(Register target)
        {
            AddInstruction(new Instruction(Opcode.Pop, target.ToString()));
        }

        public void Push(Register target)
        {
            AddInstruction(new Instruction(Opcode.Push, target.ToString()));
        }

        public void End(string label)
        {
            AddInstruction(new Instruction(Opcode.End, label));
        }

        public void Inc(Register target)
        {
            AddInstruction(new Instruction(Opcode.Inc, target.ToString()));
        }

        public void Add(Register target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Add, target.ToString() + ", " + source.ToString()));
        }

        public void Adc(Register target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Adc, target.ToString() + ", " + source.ToString()));
        }

        public void Sbc(R16Type target, R16Type source)
        {
            AddInstruction(new Instruction(Opcode.Sbc, target.ToString() + ", " + source.ToString()));
        }

        public void Ld(R8Type target, R8Type source)
        {
            AddInstruction(new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString()));
        }

        public void Ld(Register target, short source)
        {
            AddInstruction(new Instruction(Opcode.Ld, target.ToString() + ", " + source));
        }

        public void Ld(R8Type target, I16Type source, short offset)
        {
            AddInstruction(new Instruction(Opcode.Ld, target.ToString() + ", (" + source.ToString() + string.Format("{0:+#;-#;+0}", offset) + ")"));
        }

        public void Ld(I16Type target, short offset, R8Type source)
        {
            AddInstruction(new Instruction(Opcode.Ld, "(" + target.ToString() + string.Format("{0:+#;-#;+0}", offset) + "), " + source.ToString()));
        }

        public void Ld(Register target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString()));
        }

        public void Ld(R16Type target, string label)
        {
            AddInstruction(new Instruction(Opcode.Ld, target.ToString() + ", " + label));
        }

        public void LdInd(R16Type target, R8Type source)
        {
            AddInstruction(new Instruction(Opcode.Ld, "(" + target.ToString() + "), " + source.ToString()));
        }

        public void Ex(Register target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Ex, target.ToString() + ", " + source.ToString()));
        }

        public void Or(R8Type target, R8Type source)
        {
            AddInstruction(new Instruction(Opcode.Or, target.ToString() + ", " + source.ToString()));
        }

        public void Jp(Condition condition, string label)
        {
            AddInstruction(new Instruction(Opcode.Jp, condition + " , " + label));
        }

        public void Jp(string label)
        {
            AddInstruction(new Instruction(Opcode.Jp, label));
        }

        public void Db(string data, string label)
        {
            AddInstruction(new Instruction(label, Opcode.Db, $"'{data}'"));
        }

        public void Db(byte b)
        {
            AddInstruction(new Instruction(Opcode.Db, b.ToString()));
        }
    }
}
