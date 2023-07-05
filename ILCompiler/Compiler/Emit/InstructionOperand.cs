using System.Text;

namespace ILCompiler.Compiler.Emit
{
    public class InstructionOperand
    {
        public Register Register { get; set; }
        public MemoryOperand? Memory { get; set; } = null;
        public ushort? Immediate { get; set; } = null;
        public string? Data { get; set; } = null;
        public string? Label { get; set; } = null;

        public void FormatOperand(StringBuilder stringBuilder)
        {
            if (FormatMemoryOperand(stringBuilder)) return;

            if (Register != Register.None)
            {
                stringBuilder.Append(Register);
            }
            else if (Label != null)
            {
                stringBuilder.Append(Label.ToUpper());
            }
            else if (Immediate != null)
            {
                stringBuilder.Append(Immediate);
            }
            else if (Data != null)
            {
                stringBuilder.Append($"'{Data}'");
            }
        }

        private bool FormatMemoryOperand(StringBuilder stringBuilder)
        {
            if (Memory != null)
            {
                stringBuilder.Append('(');

                if (Memory.Register != Register.None)
                {
                    stringBuilder.Append(Memory.Register);

                    if (Memory.Register.IsIndexRegister())
                    {
                        if (Memory.Displacement >= 0)
                        {
                            stringBuilder.Append('+');
                        }
                        stringBuilder.Append(Memory.Displacement);
                    }
                }
                else if (Memory.Label != null)
                {
                    stringBuilder.Append(Memory.Label.ToUpper());
                }
                else
                {
                    stringBuilder.Append(Memory.Displacement);
                }

                stringBuilder.Append(')');
                return true;
            }

            return false;
        }
    }
}
