using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIndirectImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ldind_I1 || code == Code.Ldind_I2 || code == Code.Ldind_I4 || code == Code.Ldind_I;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var type = GetWellKnownType(instruction.OpCode.Code);
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = new CastEntry(WellKnownType.IntPtr, addr, VarType.Ptr);
                cast.DesiredType2 = VarType.Ptr;
                addr = cast;
            }

            var exactSize = type.GetWellKnownTypeSize();
            var desiredSize = instruction.OpCode.Code == Code.Ldind_I ? 2 : 4;

            var node = new IndirectEntry(addr, GetType(instruction.OpCode.Code), exactSize, desiredSize);
            importer.PushExpression(node);
        }

        private WellKnownType GetWellKnownType(Code code)
        {
            return code switch
            {
                Code.Ldind_I1 => WellKnownType.SByte,
                Code.Ldind_I2 => WellKnownType.Int16,
                Code.Ldind_I4 => WellKnownType.Int32,
                Code.Ldind_I => WellKnownType.Int16,
                _ => throw new NotImplementedException(),
            };
        }

        private VarType GetType(Code code)
        {
            return code switch
            {
                Code.Ldind_I1 => VarType.SByte,
                Code.Ldind_I2 => VarType.Short,
                Code.Ldind_I4 => VarType.Int,
                Code.Ldind_I => VarType.Ptr,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
