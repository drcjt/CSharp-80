using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class StoreIndirectImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stind_I1 || code == Code.Stind_I2 || code == Code.Stind_I4 || code == Code.Stind_I;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            WellKnownType type = GetWellKnownType(instruction);

            var value = importer.PopExpression();
            var addr = importer.PopExpression();

            if (value.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException();
            }

            importer.ImportAppendTree(new StoreIndEntry(addr, value, type));
        }

        private static WellKnownType GetWellKnownType(Instruction instruction)
        {
            var type = instruction.OpCode.Code switch
            {
                Code.Stind_I1 => WellKnownType.SByte,
                Code.Stind_I2 => WellKnownType.Int16,
                Code.Stind_I4 => WellKnownType.Int32,
                Code.Stind_I => WellKnownType.Int32,
                _ => throw new NotImplementedException(),
            };
            return type;
        }
    }
}
