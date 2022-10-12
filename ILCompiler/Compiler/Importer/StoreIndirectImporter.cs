using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreIndirectImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stind_I1 || code == Code.Stind_I2 || code == Code.Stind_I4 || code == Code.Stind_I;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var value = importer.PopExpression();
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = new CastEntry(addr, VarType.Ptr);
                addr = cast;
            }    

            int exactSize = GetType(instruction.OpCode.Code).GetTypeSize();

            var node = new StoreIndEntry(addr, value, fieldOffset: 0, exactSize);
            node.Type = value.Type;

            importer.ImportAppendTree(node);
        }

        private static VarType GetType(Code code)
        {
            return code switch
            {
                Code.Stind_I1 => VarType.SByte,
                Code.Stind_I2 => VarType.Short,
                Code.Stind_I4 => VarType.Int,
                Code.Stind_I => VarType.Ptr,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
