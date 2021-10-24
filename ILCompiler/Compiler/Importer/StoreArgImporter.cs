using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class StoreArgImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Starg || code == Code.Starg_S;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var value = importer.PopExpression();
            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException("Storing to argument other than short, int32 or object refs not supported yet");
            }
            var node = new StoreLocalVariableEntry((instruction.OperandAs<Parameter>()).Index, true, value);
            importer.ImportAppendTree(node);
        }
    }
}
