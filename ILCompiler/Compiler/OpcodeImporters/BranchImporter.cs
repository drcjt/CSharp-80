using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class BranchImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            var code = instruction.Opcode;
            switch (instruction.Opcode)
            {
                case ILOpcode.br:
                case ILOpcode.blt:
                case ILOpcode.blt_un:
                case ILOpcode.bgt:
                case ILOpcode.bgt_un:
                case ILOpcode.ble:
                case ILOpcode.ble_un:
                case ILOpcode.bge:
                case ILOpcode.bge_un:
                case ILOpcode.beq:
                case ILOpcode.bne_un:
                case ILOpcode.brfalse:
                case ILOpcode.brtrue:
                    break;

                case ILOpcode.br_s:
                case ILOpcode.blt_s:
                case ILOpcode.blt_un_s:
                case ILOpcode.bgt_s:
                case ILOpcode.bgt_un_s:
                case ILOpcode.ble_s:
                case ILOpcode.ble_un_s:
                case ILOpcode.bge_s:
                case ILOpcode.bge_un_s:
                case ILOpcode.beq_s:
                case ILOpcode.bne_un_s:
                case ILOpcode.brfalse_s:
                case ILOpcode.brtrue_s:
                    code += (ILOpcode.br - ILOpcode.br_s);
                    break;

                default:
                    return false;
            }

            var target = (Instruction)instruction.Operand;

            var targetBlock = importer.BasicBlocks[(int)target.Offset];
            var fallthroughBlock = (code != ILOpcode.br) ? context.FallThroughBlock : null;

            if (code != ILOpcode.br)
            {
                var op2 = importer.Pop();
                if (!op2.Type.IsInt() && op2.Type != VarType.Ptr && op2.Type != VarType.ByRef && op2.Type != VarType.Ref)
                {
                    throw new NotSupportedException($"Boolean comparisons with op2 type of {op2.Type} not supported");
                }

                StackEntry op1;
                Operation op;
                if (code != ILOpcode.brfalse && code != ILOpcode.brtrue)
                {
                    op1 = importer.Pop();
                    op = Operation.Eq + (code - ILOpcode.beq);

                    // TODO: really need to check valuetypes too as valuetype can still be int32 or int16 sized
                    if (op1.Type != VarType.Struct && op2.Type != VarType.Struct)
                    {
                        // If one of the values is a native int then cast the other to be native int too
                        if (op1.Type == VarType.Ptr && op2.Type != VarType.Ptr)
                        {
                            var cast = CodeFolder.FoldExpression(new CastEntry(op2, VarType.Ptr));
                            op2 = cast;
                        }
                        else if (op1.Type != VarType.Ptr && op2.Type == VarType.Ptr)
                        {
                            var cast = CodeFolder.FoldExpression(new CastEntry(op1, VarType.Ptr));
                            op1 = cast;
                        }
                    }
                }
                else
                {
                    op1 = ConstantEntry.CreateZeroConstantEntry(op2.Type);
                    op = (code == ILOpcode.brfalse) ? Operation.Eq : Operation.Ne_Un;
                }

                var binopType = op1.Type == VarType.Ptr || op2.Type == VarType.Ptr ? VarType.Ptr : VarType.Int;
                op1 = new BinaryOperator(op, isComparison: true, op1, op2, binopType);
                importer.ImportAppendTree(new JumpTrueEntry(targetBlock.Label, op1));
            }
            else
            {
                importer.ImportAppendTree(new JumpEntry(targetBlock.Label));
            }

            // Fall through handling
            importer.ImportFallThrough(targetBlock);

            if (fallthroughBlock != null)
            {
                importer.ImportFallThrough(fallthroughBlock);
            }

            context.StopImporting = true;

            return true;
        }
    }
}