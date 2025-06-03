using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal class LoadElemImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            VarType elemType;
            int elemSize = 0;
            switch (instruction.Opcode)
            {
                case ILOpcode.ldelem:
                    var typeDesc = (TypeDesc)instruction.Operand;

                    elemType = typeDesc.VarType;
                    elemSize = typeDesc.GetElementSize().AsInt;
                    break;

                case ILOpcode.ldelem_i:
                    elemType = VarType.Ptr;
                    break;

                case ILOpcode.ldelem_i1:
                    elemType = VarType.SByte;
                    break;

                case ILOpcode.ldelem_u1:
                    elemType = VarType.Byte;
                    break;

                case ILOpcode.ldelem_i2:
                    elemType = VarType.Short;
                    break;

                case ILOpcode.ldelem_u2:
                    elemType = VarType.UShort;
                    break;

                case ILOpcode.ldelem_i4:
                    elemType = VarType.Int;
                    break;

                case ILOpcode.ldelem_u4:
                    elemType = VarType.UInt;
                    break;

                case ILOpcode.ldelem_ref:
                    elemType = VarType.Ref;
                    break;

                default:
                    return false;
            }

            if (instruction.Opcode != ILOpcode.ldelem)
            {
                elemSize = elemType.GetTypeSize();
            }

            var op1 = importer.Pop();
            var op2 = importer.Pop();

            var boundsCheck = !context.Configuration.SkipArrayBoundsCheck;

            var node = new IndexRefEntry(op1, op2, elemSize, elemType, 2, boundsCheck);

            importer.Push(node);

            return true;
        }
    }
}
