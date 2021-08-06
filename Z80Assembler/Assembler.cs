using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Assembler
{
    public partial class Assembler
    {
        public IList<Instruction> Instructions { get; }
        ulong _currentLabelId;

        public Assembler()
        {
            Instructions = new List<Instruction>();
        }

        public void Reset()
        {
            Instructions.Clear();
        }

        public Label CreateLabel(string name = null)
        {
            _currentLabelId++;
            var label = new Label(name, _currentLabelId);
            return label;
        }

        public void AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
        }

        public void Assemble(ICodeWriter writer)
        {
        }
    }
}
