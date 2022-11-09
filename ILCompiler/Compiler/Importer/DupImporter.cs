using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class DupImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Dup;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();

            // TODO: Consider optimising this for cases where type is simple

            // Spill to new temp
            var lclNum = importer.GrabTemp(op1.Type, op1.ExactSize);
            var asg = new StoreLocalVariableEntry(lclNum, false, op1);
            importer.ImportAppendTree(asg);

            // Push new temp twice
            var node1 = new LocalVariableEntry(lclNum, op1.Type, op1.ExactSize);
            var node2 = new LocalVariableEntry(lclNum, op1.Type, op1.ExactSize);
            importer.PushExpression(node1);
            importer.PushExpression(node2);
        }
    }
}
