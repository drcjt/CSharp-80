using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class StoreVarImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Stloc ||
                   code == Code.Stloc_S ||
                   code == Code.Stloc_0 ||
                   code == Code.Stloc_1 ||
                   code == Code.Stloc_2 ||
                   code == Code.Stloc_3;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index = GetIndex(instruction);

            var value = importer.PopExpression();
            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ObjRef && value.Kind != StackValueKind.ValueType)
            {
                throw new NotSupportedException("Storing variables other than short, int32 ,object refs, or valuetypes not supported yet");
            }
            var localNumber = importer.ParameterCount + index;
            var node = new StoreLocalVariableEntry(localNumber, false, value);
            importer.ImportAppendTree(node, true);
        }

        private static int GetIndex(Instruction instruction)
        {
            var index = instruction.OpCode.Code switch
            {
                Code.Stloc or Code.Stloc_S => (instruction.OperandAs<Local>()).Index,
                _ => instruction.OpCode.Code - Code.Stloc_0,
            };
            return index;
        }
    }
}
