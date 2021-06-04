using ILCompiler.z80;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class BasicBlock
    {
        public BasicBlock Next { get; set; }
        public int StartOffset { get; set; }

        public IList<Instruction> Code { get; set; }

        public EvaluationStack<StackEntry> EntryStack { get; set; }

        public enum ImportState
        {
            Unmarked,
            IsPending
        }

        public ImportState State = ImportState.Unmarked;

        public int Id { get;  }

        private static int nextId = 0;

        public BasicBlock()
        {
            Id = nextId++;
        }
    }
}
