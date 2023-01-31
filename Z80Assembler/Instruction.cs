using System;
using System.Text;

namespace Z80Assembler
{
    public class Instruction
    {
        public string Label { get; } = String.Empty;
        public Opcode? Opcode { get; }
        public string Operands { get; } = String.Empty;

        private readonly bool _upperCase = true;

        public Instruction(string label, Opcode? opcode, string operands) : this(opcode, operands)
        {
            Label = label;
        }

        public Instruction(Opcode? opcode, string operands) : this(opcode)
        {
            Operands = operands;
        }

        public Instruction(Opcode? opcode)
        {
            Opcode = opcode;

        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Label))
            {
                stringBuilder.Append(_upperCase ? Label.ToUpper() : Label);
                stringBuilder.Append(':');
            }
            stringBuilder.Append('\t');

            if (Opcode != null)
            {
                stringBuilder.Append((_upperCase ? Opcode.ToString().ToUpper() : Opcode) + " ");
                if (Operands != null)
                {
                    stringBuilder.Append(_upperCase ? Operands.ToUpper() : Operands);
                }
            }

            return stringBuilder.ToString();
        }

        public static Instruction Ret()
        {
            return new Instruction(Opcode.Ret);
        }

        public static Instruction Org(ushort address)
        {
            return new Instruction(Opcode.Org, string.Format("{0:X}H", address));
        }

        public static Instruction Halt()
        {
            return new Instruction(Opcode.Halt);
        }

        public static Instruction Call(string label)
        {
            return new Instruction(Opcode.Call, label);
        }

        public static Instruction Call(UInt16 target)
        {
            return new Instruction(Opcode.Call, string.Format("{0:X}H", target));
        }

        public static Instruction Pop(Register target)
        {
            return new Instruction(Opcode.Pop, target.ToString());
        }

        public static Instruction Push(Register target)
        {
            return new Instruction(Opcode.Push, target.ToString());
        }

        public static Instruction End(string label)
        {
            return new Instruction(Opcode.End, label);
        }

        public static Instruction Inc(Register target)
        {
            return new Instruction(Opcode.Inc, target.ToString());
        }

        public static Instruction Add(Register target, Register source)
        {
            return new Instruction(Opcode.Add, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Adc(Register target, Register source)
        {
            return new Instruction(Opcode.Adc, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Sbc(R16Type target, R16Type source)
        {
            return new Instruction(Opcode.Sbc, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Ld(R8Type target, R8Type source)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Ld(Register target, short source)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", " + source);
        }

        public static Instruction Ld(R8Type target, I16Type source, short offset)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", (" + source.ToString() + string.Format("{0:+#;-#;+0}", offset) + ")");
        }

        public static Instruction Ld(I16Type target, short offset, R8Type source)
        {
            return new Instruction(Opcode.Ld, "(" + target.ToString() + string.Format("{0:+#;-#;+0}", offset) + "), " + source.ToString());
        }

        public static Instruction Ld(Register target, Register source)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Ld(R16Type target, string label)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", " + label);
        }

        public static Instruction LdInd(R16Type target, R8Type source)
        {
            return new Instruction(Opcode.Ld, "(" + target.ToString() + "), " + source.ToString());
        }

        public static Instruction LdInd(Register target, string source)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", (" + source + ")");
        }

        public static Instruction LdInd(string label, Register source)
        {
            return new Instruction(Opcode.Ld, "(" + label + "), " + source.ToString());
        }

        public static Instruction Ex(Register target, Register source)
        {
            return new Instruction(Opcode.Ex, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Or(R8Type target, R8Type source)
        {
            return new Instruction(Opcode.Or, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Jp(Condition condition, string label)
        {
            return new Instruction(Opcode.Jp, condition + " , " + label);
        }

        public static Instruction Jp(string label)
        {
            return new Instruction(Opcode.Jp, label);
        }

        public static Instruction Db(string data, string label = "")
        {
            return new Instruction(label, Opcode.Db, $"'{data}'");
        }

        public static Instruction Db(byte b)
        {
            return new Instruction(Opcode.Db, b.ToString());
        }

        public static Instruction Defs(int size)
        {
            return new Instruction(Opcode.Defs, size.ToString());
        }

        public static Instruction Dc(int count, int value)
        {
            return new Instruction(Opcode.Dc, count.ToString() + ", " + value.ToString());
        }

    }
}
