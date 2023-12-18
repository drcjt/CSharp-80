using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class ThrowImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Throw) return false;

            // exception to throw
            var op1 = importer.PopExpression();

            // Create call to helper method passing exception object
            var args = new List<StackEntry>() { op1 };
            var node = new CallEntry("ThrowEx", args, VarType.Void, 0);

            importer.ImportAppendTree(node);

            return true;
        }

    }
}
