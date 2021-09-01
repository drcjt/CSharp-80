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
        public bool CanImport(Code code) => code == Code.Ldind_I1 || code == Code.Ldind_I2 || code == Code.Ldind_I4;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var type = GetWellKnownType(instruction.OpCode.Code);
            var addr = importer.PopExpression();
            var node = new IndirectEntry(addr, StackValueKind.Int32, type);
            importer.PushExpression(node);
        }

        private WellKnownType GetWellKnownType(Code code)
        {
            return code switch
            {
                Code.Ldind_I1 => WellKnownType.SByte,
                Code.Ldind_I2 => WellKnownType.Int16,
                Code.Ldind_I4 => WellKnownType.Int32,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
