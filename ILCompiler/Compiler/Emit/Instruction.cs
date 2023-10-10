using System.Text;

namespace ILCompiler.Compiler.Emit
{
    public class Instruction
    {
        public Opcode Opcode { get; private set; }

        public Condition Condition { get; private set; }
        public InstructionOperand? Op0 { get; private set; }
        public InstructionOperand? Op1 { get; private set; }

        public string? Label { get; private set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            if (Label != null)
            {
                stringBuilder.Append(Label.ToUpper());
                stringBuilder.Append(':');
            }

            stringBuilder.Append('\t');

            FormatOpcode(stringBuilder);

            return stringBuilder.ToString();
        }

        private void FormatOpcode(StringBuilder stringBuilder)
        {
            if (Opcode != Opcode.None)
            {
                stringBuilder.Append(Opcode.ToString().ToUpper());
                stringBuilder.Append(' ');

                if (Condition != Condition.None)
                {
                    stringBuilder.Append(Condition);
                    stringBuilder.Append(',');
                }
                if (Op0 != null)
                {
                    Op0.FormatOperand(stringBuilder);
                }

                if (Op1 != null)
                {
                    stringBuilder.Append(", ");

                    Op1.FormatOperand(stringBuilder);
                }
            }
        }

        public static Instruction Create(string label)
            => new() { Label = label };
        public static Instruction Create(Opcode opcode) 
            => new() { Opcode = opcode };

        public static Instruction Create(Opcode opcode, Register register) 
            => new() { Opcode = opcode, Op0 = new() { Register = register } };

        public static Instruction Create(Opcode opcode, Register target, Register source) 
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Register = source } };
        public static Instruction Create(Opcode opcode, Register target, ushort source)
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Immediate = source } };

        public static Instruction Create(Opcode opcode, string target)
            => new() { Opcode = opcode, Op0 = new() { Label = target } };

        public static Instruction Create(Opcode opcode, ushort target)
            => new() { Opcode = opcode, Op0 = new() { Immediate = target } };

        public static Instruction Create(Opcode opcode, MemoryOperand target)
            => new() { Opcode = opcode, Op0 = new() { Memory = target } };

        public static Instruction Create(Opcode opcode, MemoryOperand target, Register source) 
            => new() { Opcode = opcode, Op0 = new() { Memory = target }, Op1 = new() { Register = source } };
        public static Instruction Create(Opcode opcode, MemoryOperand target, byte source)
            => new() { Opcode = opcode, Op0 = new() { Memory = target }, Op1 = new() { Immediate = source } };

        public static Instruction Create(Opcode opcode, Register target, MemoryOperand source) 
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Memory = source } };

        public static Instruction Create(Opcode opcode, Register target, string source) 
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Label = source } };
        public static Instruction Create(Opcode opcode, ushort count, ushort value)
            => new() { Opcode = opcode, Op0 = new() { Immediate = count }, Op1 = new() { Immediate = value } };

        public static Instruction CreateBranch(Opcode opcode, string target)
    => new() { Opcode = opcode, Op0 = new() { Label = target } };
        public static Instruction CreateBranch(Opcode opcode, Condition condition, string target)
            => new() { Opcode = opcode, Condition = condition, Op0 = new() { Label = target } };
        public static Instruction CreateBranch(Opcode opcode, ushort target)
            => new() { Opcode = opcode, Op0 = new() { Immediate = target } };

        public static Instruction CreateDeclareByte(Opcode opcode, string data, string label)
            => new() { Label = label, Opcode = opcode, Op0 = new() { Data = data } };
        public static Instruction CreateDeclareByte(Opcode opcode, string data)
            => new() { Opcode = opcode, Op0 = new() { Data = data } };

        public static Instruction CreateDeclareWord(Opcode opcode, string label)
            => new() { Opcode = opcode, Op0 = new() { Label = label } };
    }
}
