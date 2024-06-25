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

        public string? Comment { get; private set; }

        public int Bytes { get; private set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            if (Label != null)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(Label.ToUpper());
                stringBuilder.Append(':');
            }

            if (Opcode != Opcode.None)
            {
                stringBuilder.Append('\t');
            }

            FormatOpcode(stringBuilder);

            AppendComment(stringBuilder);

            return stringBuilder.ToString();
        }

        private void AppendComment(StringBuilder stringBuilder)
        {
            const int MaxLineLength = 120;

            if (!string.IsNullOrEmpty(Comment))
            {
                if (Opcode != Opcode.None)
                {
                    stringBuilder.Append(" ");
                }

                var comment = Comment;
                do
                {
                    stringBuilder.Append(";");
                    var chunkLength = Math.Min(MaxLineLength, comment.Length);
                    stringBuilder.Append(comment.AsSpan(0, chunkLength));
                    comment = comment.Substring(chunkLength);
                    if (comment.Length > 0)
                    {
                        stringBuilder.AppendLine();
                    }

                } while (comment.Length > 0);
            }
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
        public static Instruction Create(Opcode opcode, int bytes = 1) 
            => new() { Opcode = opcode, Bytes = bytes };

        public static Instruction Create(Opcode opcode, Register register, int bytes = 1) 
            => new() { Opcode = opcode, Op0 = new() { Register = register }, Bytes = bytes };

        public static Instruction Create(Opcode opcode, Register target, Register source, int bytes = 1) 
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Register = source }, Bytes = bytes };
        public static Instruction Create(Opcode opcode, Register target, ushort source, int bytes = 1)
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Immediate = source }, Bytes = bytes };
        public static Instruction Create(Opcode opcode, string target)
            => new() { Opcode = opcode, Op0 = new() { Label = target } };

        public static Instruction Create(Opcode opcode, string label, ushort value)
            => new() { Opcode = opcode, Label = label, Op0 = new() { Immediate = value }, Bytes = 2 };

        public static Instruction Create(Opcode opcode, ushort target, string? comment = null, int bytes = 1)
            => new() { Opcode = opcode, Op0 = new() { Immediate = target }, Comment = comment, Bytes = bytes };

        public static Instruction Create(Opcode opcode, MemoryOperand target, int bytes = 1)
            => new() { Opcode = opcode, Op0 = new() { Memory = target }, Bytes = bytes };

        public static Instruction Create(Opcode opcode, MemoryOperand target, Register source, int bytes = 1) 
            => new() { Opcode = opcode, Op0 = new() { Memory = target }, Op1 = new() { Register = source }, Bytes = bytes };
        public static Instruction Create(Opcode opcode, MemoryOperand target, byte source)
            => new() { Opcode = opcode, Op0 = new() { Memory = target }, Op1 = new() { Immediate = source }, Bytes = target.Register.IsIndexRegister() ? 4 : 2 };

        public static Instruction Create(Opcode opcode, Register target, MemoryOperand source, int bytes = 1) 
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Memory = source }, Bytes = bytes };

        public static Instruction Create(Opcode opcode, Register target, string source, int bytes = 3) 
            => new() { Opcode = opcode, Op0 = new() { Register = target }, Op1 = new() { Label = source }, Bytes = bytes };
        public static Instruction Create(Opcode opcode, ushort count, ushort value)
            => new() { Opcode = opcode, Op0 = new() { Immediate = count }, Op1 = new() { Immediate = value }, Bytes = count };

        public static Instruction CreateComment(string comment)
            => new() { Opcode = Opcode.None, Comment = comment };

        public static Instruction CreateBranch(Opcode opcode, string target, int bytes = 3)
            => new() { Opcode = opcode, Op0 = new() { Label = target }, Bytes = bytes };
        public static Instruction CreateBranch(Opcode opcode, Condition condition, string target, int bytes = 3)
            => new() { Opcode = opcode, Condition = condition, Op0 = new() { Label = target }, Bytes = bytes };
        public static Instruction CreateBranch(Opcode opcode, ushort target)
            => new() { Opcode = opcode, Op0 = new() { Immediate = target }, Bytes = 3 };

        public static Instruction CreateDeclareByte(Opcode opcode, string data, string label)
            => new() { Label = label, Opcode = opcode, Op0 = new() { Data = data }, Bytes = data.Length };
        public static Instruction CreateDeclareByte(Opcode opcode, string data)
            => new() { Opcode = opcode, Op0 = new() { Data = data }, Bytes = data.Length };

        public static Instruction CreateDeclareWord(Opcode opcode, string label, string? comment = null)
            => new() { Opcode = opcode, Op0 = new() { Label = label }, Comment = comment, Bytes = 2 };
    }
}
