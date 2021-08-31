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
        private readonly IILImporter _importer;
        public StoreIndirectImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Stind_I1 || opcode == Code.Stind_I2 || opcode == Code.Stind_I4;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            WellKnownType type;
            switch (instruction.OpCode.Code)
            {
                case Code.Stind_I1:
                    type = WellKnownType.SByte;
                    break;
                case Code.Stind_I2:
                    type = WellKnownType.Int16;
                    break;
                case Code.Stind_I4:
                    type = WellKnownType.Int32;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var value = _importer.PopExpression();
            var addr = _importer.PopExpression();

            if (value.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException();
            }

            _importer.ImportAppendTree(new StoreIndEntry(addr, value, type));
        }
    }
}
