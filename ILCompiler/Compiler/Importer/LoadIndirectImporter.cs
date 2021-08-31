using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIndirectImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public LoadIndirectImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldind_I1 || opcode == Code.Ldind_I2 || opcode == Code.Ldind_I4;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            WellKnownType type;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldind_I1:
                    type = WellKnownType.SByte;
                    break;
                case Code.Ldind_I2:
                    type = WellKnownType.Int16;
                    break;
                case Code.Ldind_I4:
                    type = WellKnownType.Int32;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var addr = _importer.PopExpression();
            var node = new IndirectEntry(addr, StackValueKind.Int32, type);
            _importer.PushExpression(node);
        }
    }
}
