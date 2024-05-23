using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIndirectImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType type;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldind_I:
                    type = VarType.Ptr;
                    break;
                case Code.Ldind_I1:
                    type = VarType.SByte;
                    break;
                case Code.Ldind_U1:
                    type = VarType.Byte;
                    break;
                case Code.Ldind_I2:
                    type = VarType.Short;
                    break;
                case Code.Ldind_U2:
                    type = VarType.UShort;
                    break;
                case Code.Ldind_U4:
                    type = VarType.UInt;
                    break;
                case Code.Ldind_I4:
                    type = VarType.Int;
                    break;
                case Code.Ldind_Ref:
                    type = VarType.Ref;
                    break;
                case Code.Ldobj:
                    var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
                    var typeDesc = context.TypeSystemContext.Create(typeSig, context.Method.Instantiation);
                    type = typeDesc.VarType;
                    break;

                default:
                    return false;
            }
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = CodeFolder.FoldExpression(new CastEntry(addr, VarType.Ptr));
                addr = cast;
            }

            var exactSize = type.GetTypeSize();

            var node = new IndirectEntry(addr, type, exactSize);
            importer.PushExpression(node);

            return true;
        }
    }
}
