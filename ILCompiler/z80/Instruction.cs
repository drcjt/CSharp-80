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

        public Instruction(Opcode opcode, string operands)
        {
            this.Opcode = opcode;
            this.Operands = operands;
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
                stringBuilder.Append(_upperCase ? Operands.ToUpper() : Operands);
            }

			return stringBuilder.ToString();
		}
	}
}
