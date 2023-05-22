using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class BranchImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var code = instruction.OpCode.Code;
            switch (instruction.OpCode.Code)
            {
                case Code.Br:
                case Code.Blt:
                case Code.Blt_Un:
                case Code.Bgt:
                case Code.Bgt_Un:
                case Code.Ble:
                case Code.Ble_Un:
                case Code.Bge:
                case Code.Bge_Un:
                case Code.Beq:
                case Code.Bne_Un:
                case Code.Brfalse:
                case Code.Brtrue:
                    break;

                case Code.Br_S:
                case Code.Blt_S:
                case Code.Blt_Un_S:
                case Code.Bgt_S:
                case Code.Bgt_Un_S:
                case Code.Ble_S:
                case Code.Ble_Un_S:
                case Code.Bge_S:
                case Code.Bge_Un_S:
                case Code.Beq_S:
                case Code.Bne_Un_S:
                case Code.Brfalse_S:
                case Code.Brtrue_S:
                    code += (Code.Br - Code.Br_S);
                    break;

                default:
                    return false;
            }

            var target = instruction.OperandAs<Instruction>();

            var targetBlock = importer.BasicBlocks[(int)target.Offset];
            var fallthroughBlock = (code != Code.Br) ? context.FallThroughBlock : null;

            if (code != Code.Br)
            {
                var op2 = importer.PopExpression();
                if (!op2.Type.IsInt() && op2.Type != VarType.Ptr && op2.Type != VarType.ByRef && op2.Type != VarType.Ref)
                {
                    throw new NotSupportedException($"Boolean comparisons with op2 type of {op2.Type} not supported");
                }

                StackEntry op1;
                Operation op;
                if (code != Code.Brfalse && code != Code.Brtrue)
                {
                    op1 = importer.PopExpression();
                    op = Operation.Eq + (code - Code.Beq);

                    // TODO: really need to check valuetypes too as valuetype can still be int32 or int16 sized
                    if (op1.Type != VarType.Struct && op2.Type != VarType.Struct)
                    {
                        // If one of the values is a native int then cast the other to be native int too
                        if (op1.Type == VarType.Ptr && op2.Type != VarType.Ptr)
                        {
                            var cast = new CastEntry(op2, VarType.Ptr);
                            op2 = cast;
                        }
                        else if (op1.Type != VarType.Ptr && op2.Type == VarType.Ptr)
                        {
                            var cast = new CastEntry(op1, VarType.Ptr);
                            op1 = cast;
                        }
                    }
                }
                else
                {
                    op1 = new Int32ConstantEntry(0);
                    op = (code == Code.Brfalse) ? Operation.Eq : Operation.Ne_Un;
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
