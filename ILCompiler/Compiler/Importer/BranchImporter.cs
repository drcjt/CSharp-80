using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

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

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
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
            var target = instruction.Operand as Instruction;

            var targetBlock = importer.BasicBlocks[(int)target.Offset];
            var fallthroughBlock = (code != Code.Br) ? context.CurrentBlock : null;

            if (code != Code.Br)
            {
                var op2 = importer.PopExpression();
                if (op2.Kind != StackValueKind.Int32)
                {
                    throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
                }

                StackEntry op1;
                Operation op;
                if (code != Code.Brfalse && code != Code.Brtrue)
                {
                    op1 = importer.PopExpression();
                    if (op2.Kind != StackValueKind.Int32)
                    {
                        throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
                    }
                    op = Operation.Eq + (code - Code.Beq);
                }
                else
                {
                    op1 = new Int32ConstantEntry((short)(code == Code.Brfalse ? 0 : 1));
                    op = Operation.Eq;
                }
                op1 = new BinaryOperator(op, op1, op2, StackValueKind.Int32);
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
