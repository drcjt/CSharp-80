namespace ILCompiler.Compiler.Emit
{
    public partial class Emitter
    {
        public void Ret()
        {
            AddInstruction(new Instruction(Opcode.Ret));
        }

        public void Call(string label)
        {
            AddInstruction(new Instruction(Opcode.Call, label));
        }

        public void Call(UInt16 target)
        {
            AddInstruction(new Instruction(Opcode.Call, string.Format("{0:X}H", target)));
        }

        public void Rst(UInt16 target)
        {
            AddInstruction(new Instruction(Opcode.Rst, string.Format("{0:X}H", target)));
        }


        public void Pop(Register target)
        {
            AddInstruction(new Instruction(Opcode.Pop, target.ToString()));
        }

        public void Halt()
        {
            AddInstruction(new Instruction(Opcode.Halt));
        }

        public void Exx()
        {
            AddInstruction(new Instruction(Opcode.Exx));
        }

        public void Push(Register target)
        {
            AddInstruction(new Instruction(Opcode.Push, target.ToString()));
        }

        public void Dec(Register target)
        {
            AddInstruction(new Instruction(Opcode.Dec, target.ToString()));
        }

        public void Add(Register target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Add, target.ToString() + ", " + source.ToString()));
        }

        public void Adc(Register target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Adc, target.ToString() + ", " + source.ToString()));
        }

        public void Sbc(R16Type target, Register source)
        {
            AddInstruction(new Instruction(Opcode.Sbc, target.ToString() + ", " + source.ToString()));
        }

        public void Sbc(R8Type target, R8Type source)
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
            if (offset < -128 || offset > 127)
            {
                throw new NotImplementedException($"Index addressing offset {offset} not supported");
            }
            AddInstruction(new Instruction(Opcode.Ld, target.ToString() + ", (" + source.ToString() + string.Format("{0:+#;-#;+0}", offset) + ")"));
        }

        public void Ld(I16Type target, short offset, R8Type source)
        {
            if (offset < -128 || offset > 127)
            {
                throw new NotImplementedException($"Index addressing offset {offset} not supported");
            }
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

        public void Or(R8Type target)
        {
            AddInstruction(new Instruction(Opcode.Or, target.ToString()));
        }

        public void RotateRight(R8Type target)
        {
            AddInstruction(new Instruction(Opcode.Rr, target.ToString()));
        }
        public void ShiftRightLogical(R8Type target)
        {
            AddInstruction(new Instruction(Opcode.Srl, target.ToString()));
        }

        public void And(R8Type target)
        {
            AddInstruction(new Instruction(Opcode.And, target.ToString()));
        }

        public void Jp(Condition condition, string label)
        {
            AddInstruction(new Instruction(Opcode.Jp, condition + " , " + label));
        }

        public void Jp(string label)
        {
            AddInstruction(new Instruction(Opcode.Jp, label));
        }

        public void Db(byte b)
        {
            AddInstruction(new Instruction(Opcode.Db, b.ToString()));
        }
    }
}
