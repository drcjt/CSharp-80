using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ILCompiler.z80
{
	public partial class Assembly
    {
		public Assembly()
		{
		}

		private IList<Instruction> _instructions = new List<Instruction>();

		public Instruction this[int index]
		{
			get
			{
				return _instructions[index];
			}
		}

		public void LABEL(string label)
		{
			_instructions.Add(new LabelInstruction(label));
		}

		public void END(string label)
        {
			_instructions.Add(new Instruction(string.Empty, "END", label)); 
        }

		public void Write(string filePath, string inputFilePath)
		{
			using (StreamWriter streamWriter = new StreamWriter(filePath))
			{
				streamWriter.WriteLine($"; INPUT FILE {inputFilePath.ToUpper()}");
				streamWriter.WriteLine($"; {DateTime.Now}");
				streamWriter.WriteLine();
				streamWriter.WriteLine("\tORG 5200H");

				foreach (Instruction instruction in _instructions)
				{
					streamWriter.WriteLine(instruction.ToString());
				}
			}
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			foreach (Instruction instruction in _instructions)
			{
				stringBuilder.Append(instruction.ToString());
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
		}
	}
}
