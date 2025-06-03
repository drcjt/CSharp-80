using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal class ThrowImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.throw_) return false;

            // exception to throw
            var op1 = importer.Pop();

            // Create call to helper method passing exception object
            var args = new List<StackEntry>() { op1 };
            var node = new CallEntry(GetThrowHelper(), args, VarType.Void, 0);

            importer.ImportAppendTree(node);

            return true;
        }

        public static string GetThrowHelper()
        {
            // If no exception handlers then can just use fail fast
            // and avoid any overhead of searching for exception handler
            if (!Compilation.AnyExceptionHandlers)
            {
                return "FailFast";
            }

            return "ThrowEx";
        }
    }
}