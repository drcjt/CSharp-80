using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreIndirectImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType type;
            int exactSize = 0;
            switch (instruction.OpCode.Code)
            {
                case Code.Stind_I1:
                    type = VarType.SByte;
                    break;

                case Code.Stind_I2:
                    type = VarType.Short;
                    break;

                case Code.Stind_I4:
                    type = VarType.Int;
                    break;

                case Code.Stind_I:
                    type = VarType.Ptr;
                    break;

                case Code.Stind_Ref:
                    type = VarType.Ref;
                    break;

                case Code.Stobj:
                    var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
                    typeSig = context.Method.ResolveType(typeSig);
                    type = typeSig.GetVarType();
                    exactSize  = typeSig.GetInstanceFieldSize();
                    break;

                default:
                    return false;
            }

            if (instruction.OpCode.Code != Code.Stobj)
            {
                exactSize = type.GetTypeSize();
            }

            var value = importer.PopExpression();
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = CodeFolder.FoldExpression(new CastEntry(addr, VarType.Ptr));
                addr = cast;
            }    

            if (type.IsSmall() && !value.Type.IsSmall())
            {
                value = CodeFolder.FoldExpression(new CastEntry(value, type));
            }

            var node = new StoreIndEntry(addr, value, value.Type, fieldOffset: 0, exactSize);

            importer.ImportAppendTree(node);

            return true;
        }
    }
}
