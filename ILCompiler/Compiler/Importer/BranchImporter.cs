using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class BranchImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            switch (code)
            {
                case Code.Br_S:
                case Code.Blt_S:
                case Code.Bgt_S:
                case Code.Ble_S:
                case Code.Bge_S:
                case Code.Beq_S:
                case Code.Bne_Un_S:
                case Code.Brfalse_S:
                case Code.Brtrue_S:

                case Code.Br:
                case Code.Blt:
                case Code.Bgt:
                case Code.Ble:
                case Code.Bge:
                case Code.Beq:
                case Code.Bne_Un:
                case Code.Brfalse:
                case Code.Brtrue:
                    return true;
            }

            return false;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var code = instruction.OpCode.Code;
            switch (code)
            {
                case Code.Br_S:
                case Code.Blt_S:
                case Code.Bgt_S:
                case Code.Ble_S:
                case Code.Bge_S:
                case Code.Beq_S:
                case Code.Bne_Un_S:
                case Code.Brfalse_S:
                case Code.Brtrue_S:
                    code += (Code.Br - Code.Br_S);
                    break;
            }
            var target = instruction.OperandAs<Instruction>();

            var targetBlock = importer.BasicBlocks[(int)target.Offset];
            var fallthroughBlock = (code != Code.Br) ? context.FallThroughBlock : null;

            if (code != Code.Br)
            {
                var op2 = importer.PopExpression();
                if (op2.Kind != StackValueKind.Int32 && op2.Kind != StackValueKind.NativeInt 
                    && op2.Kind != StackValueKind.ByRef && op2.Kind != StackValueKind.ObjRef)
                {
                    throw new NotSupportedException($"Boolean comparisons with op2 kind of {op2.Kind} not supported");
                }

                StackEntry op1;
                Operation op;
                if (code != Code.Brfalse && code != Code.Brtrue)
                {
                    op1 = importer.PopExpression();
                    op = Operation.Eq + (code - Code.Beq);

                    if (op1.Kind != StackValueKind.ValueType && op2.Kind != StackValueKind.ValueType)
                    {
                        // If one of the values is a native int then cast the other to be native int too
                        if (TypeList.GetExactSize(op1.Kind) == 2 && TypeList.GetExactSize(op2.Kind) == 4)
                        {
                            op2 = new CastEntry(Common.TypeSystem.WellKnownType.Object, op2, op1.Kind);
                        }
                        else if (TypeList.GetExactSize(op1.Kind) == 4 && TypeList.GetExactSize(op2.Kind) == 2)
                        {
                            op1 = new CastEntry(Common.TypeSystem.WellKnownType.Object, op1, op2.Kind);
                        }
                    }
                }
                else
                {
                    op1 = new Int32ConstantEntry(0);
                    op = (code == Code.Brfalse) ? Operation.Eq : Operation.Ne;
                }
                op1 = new BinaryOperator(op, isComparison: true, op1, op2, op1.Kind);
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
        }
    }
}
