using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType elemType;
            int elemSize = 0;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldelem:
                    var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
                    var typeDesc = context.Module.Create(typeSig, context.Method.Instantiation);
                    elemType = typeDesc.VarType;
                    elemSize = typeDesc.GetElementSize().AsInt;
                    break;

                case Code.Ldelem_I:
                    elemType = VarType.Ptr;
                    break;

                case Code.Ldelem_I1:
                    elemType = VarType.SByte;
                    break;

                case Code.Ldelem_U1:
                    elemType = VarType.Byte;
                    break;

                case Code.Ldelem_I2:
                    elemType = VarType.Short;
                    break;

                case Code.Ldelem_U2:
                    elemType = VarType.UShort;
                    break;

                case Code.Ldelem_I4:
                    elemType = VarType.Int;
                    break;

                case Code.Ldelem_U4:
                    elemType = VarType.UInt;
                    break;

                case Code.Ldelem_Ref:
                    elemType = VarType.Ref;
                    break;

                default:
                    return false;
            }

            if (instruction.OpCode.Code != Code.Ldelem)
            {
                elemSize = elemType.GetTypeSize();
            }

            var op1 = importer.PopExpression();
            var op2 = importer.PopExpression();

            var boundsCheck = !context.Configuration.SkipArrayBoundsCheck;

            var node = new IndexRefEntry(op1, op2, elemSize, elemType, 2, boundsCheck);

            importer.PushExpression(node);

            return true;
        }
    }
}
