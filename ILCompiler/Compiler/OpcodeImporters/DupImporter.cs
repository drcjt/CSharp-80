using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class DupImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.dup) return false;

            var op1 = importer.Pop();

            // TODO: Consider optimising this for cases where type is simple

            // Spill to new temp
            var lclNum = importer.GrabTemp(op1.Type, op1.ExactSize);
            var asg = new StoreLocalVariableEntry(lclNum, false, op1);
            importer.ImportAppendTree(asg);

            // Push new temp twice
            var node1 = new LocalVariableEntry(lclNum, op1.Type, op1.ExactSize);
            var node2 = new LocalVariableEntry(lclNum, op1.Type, op1.ExactSize);
            importer.Push(node1);
            importer.Push(node2);

            return true;
        }
    }
}
