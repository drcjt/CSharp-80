using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ILCompiler.z80
{
	public class Z80Assembly : IZ80Assembly
    {
		public Z80Assembly()
		{
		}

		private IList<Z80Instruction> _instructions = new List<Z80Instruction>();

		public void Add(Z80Instruction instruction)
        {
			_instructions.Add(instruction);
        }

		public void RemoveLast()
        {
			_instructions.RemoveAt(_instructions.Count - 1);
		}

		public Z80Instruction Last
        {
			get
            {
				return _instructions[^1];
            }
        }

		public Z80Instruction this[int index]
		{
			get
			{
				return _instructions[index];
			}
		}

		public void Label(string label)
		{
			_instructions.Add(new LabelInstruction(label));
		}

		public void Write(string filePath, string inputFilePath)
		{
			using (StreamWriter streamWriter = new StreamWriter(filePath))
			{
				streamWriter.WriteLine($"; INPUT FILE {inputFilePath.ToUpper()}");
				streamWriter.WriteLine($"; {DateTime.Now}");
				streamWriter.WriteLine();

				foreach (Z80Instruction instruction in _instructions)
				{
					streamWriter.WriteLine(instruction.ToString());
				}
			}
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			foreach (Z80Instruction instruction in _instructions)
			{
				stringBuilder.Append(instruction.ToString());
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
		}
	}
}
