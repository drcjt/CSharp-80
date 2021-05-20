using System.Text;

namespace ILCompiler.z80
{
    public class Instruction
    {
        public Instruction(string label, string opcode, string operands, string comment)
        {
            this.Label = label;
            this.Opcode = opcode;
            this.Operands = operands;
            this.Comment = comment;
        }

        public string Label { get; }
        public string Opcode { get; }
        public string Operands { get; }
        public string Comment { get; }

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Label))
            {
                stringBuilder.Append(Label.ToUpper());
                stringBuilder.Append(':');
            }

            stringBuilder.Append('\t');
			stringBuilder.Append(Opcode + " ");
			stringBuilder.Append(Operands);

			return stringBuilder.ToString();
		}
	}
}
