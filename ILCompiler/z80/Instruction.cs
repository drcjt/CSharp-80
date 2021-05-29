﻿using System;
using System.Text;

namespace ILCompiler.z80
{
    public class Instruction
    {
        public string Label { get; }
        public Opcode Opcode { get; }
        public string Operands { get; }

        private readonly bool _upperCase = true;

        public Instruction(string label, Opcode opcode, string operands) : this(opcode, operands)
        {
            this.Label = label;
        }

        public Instruction(Opcode opcode, string operands) : this(opcode)
        {
            this.Operands = operands;
        }

        public Instruction(Opcode opcode)
        {
            this.Opcode = opcode;
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

        public static Instruction Org(short address)
        {
            return new Instruction(Opcode.Org, string.Format("{0:X}H", address));
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

        public static Instruction Add(Register target, Register source)
        {
            return new Instruction(Opcode.Add, target.ToString() + ", " + source.ToString());
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
            return new Instruction(Opcode.Ld, target.ToString() + ", " + string.Format("{0:X}H", source));
        }

        public static Instruction Ld(R8Type target, I16Type source, short offset)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", (" + source.ToString() + string.Format("{0:+#;-#;+0}", offset) + ")");
        }

        public static Instruction Ld(R16Type target, R16Type source)
        {
            return new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString());
        }
        public static Instruction Ex(Register target, Register source)
        {
            return new Instruction(Opcode.Ex, target.ToString() + ", " + source.ToString());
        }

        public static Instruction Or(R8Type target)
        {
            return new Instruction(Opcode.Or, target.ToString());
        }

        public static Instruction Jp(Condition condition, string label)
        {
            return new Instruction(Opcode.Jp, condition + " , " + label);
        }
    }
}
