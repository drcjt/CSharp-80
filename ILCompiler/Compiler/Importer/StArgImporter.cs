using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class StArgImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public StArgImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Starg ||
                   opcode == Code.Starg_S;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var parameter = instruction.Operand as Parameter;
            var index = parameter.Index;

            var value = _importer.PopExpression();
            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException("Storing to argument other than short, int32 or object refs not supported yet");
            }
            var node = new StoreLocalVariableEntry(index, true, value);
            _importer.ImportAppendTree(node);
        }
    }
}
