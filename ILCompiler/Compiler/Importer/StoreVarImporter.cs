using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class StoreVarImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public StoreVarImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Stloc ||
                   opcode == Code.Stloc_S ||
                   opcode == Code.Stloc_0 ||
                   opcode == Code.Stloc_1 ||
                   opcode == Code.Stloc_2 ||
                   opcode == Code.Stloc_3;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var index = 0;
            switch (instruction.OpCode.Code)
            {
                case Code.Stloc:
                case Code.Stloc_S:
                    index = (instruction.Operand as Local).Index;
                    break;

                default:
                    index = instruction.OpCode.Code - Code.Stloc_0;
                    break;
            }

            var value = _importer.PopExpression();
            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ObjRef && value.Kind != StackValueKind.ValueType)
            {
                throw new NotSupportedException("Storing variables other than short, int32 ,object refs, or valuetypes not supported yet");
            }
            var localNumber = _importer.ParameterCount + index;
            var node = new StoreLocalVariableEntry(localNumber, false, value);
            _importer.ImportAppendTree(node);
        }
    }
}
