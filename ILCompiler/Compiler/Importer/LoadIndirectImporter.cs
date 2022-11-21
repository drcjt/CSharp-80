using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIndirectImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ldind_I1 || code == Code.Ldind_I2 || code == Code.Ldind_I4 || code == Code.Ldind_I || code == Code.Ldind_Ref;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var type = GetType(instruction.OpCode.Code);
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = new CastEntry(addr, VarType.Ptr);
                addr = cast;
            }

            var exactSize = type.GetTypeSize();


            var node = new IndirectEntry(addr, GetType(instruction.OpCode.Code), exactSize);
            importer.PushExpression(node);
        }

        private static VarType GetType(Code code)
        {
            return code switch
            {
                Code.Ldind_I1 => VarType.SByte,
                Code.Ldind_I2 => VarType.Short,
                Code.Ldind_I4 => VarType.Int,
                Code.Ldind_I => VarType.Ptr,
                Code.Ldind_Ref => VarType.Ref,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
