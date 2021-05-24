﻿using System;
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

		private IList<Instruction> _instructions = new List<Instruction>();

		public void Add(Instruction instruction)
        {
			_instructions.Add(instruction);
        }

		public void RemoveLast()
        {
			_instructions.RemoveAt(_instructions.Count - 1);
		}

		public Instruction Last
        {
			get
            {
				return _instructions[^1];
            }
        }

		public Instruction this[int index]
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

				foreach (Instruction instruction in _instructions)
				{
					streamWriter.WriteLine(instruction.ToString());
				}

				streamWriter.WriteLine();	// Z80Asm seems to need this
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
