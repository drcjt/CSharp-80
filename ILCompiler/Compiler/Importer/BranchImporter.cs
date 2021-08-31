using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class BranchImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public BranchImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            switch (opcode)
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

        public void Import(Instruction instruction, ImportContext context)
        {
            var opcode = instruction.OpCode.Code;
            switch (opcode)
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
                    opcode += (Code.Br - Code.Br_S);
                    break;
            }
            var target = instruction.Operand as Instruction;

            var targetBlock = _importer.BasicBlocks[(int)target.Offset];
            var fallthroughBlock = (opcode != Code.Br) ? context.CurrentBlock : null;

            if (opcode != Code.Br)
            {
                var op2 = _importer.PopExpression();
                if (op2.Kind != StackValueKind.Int32)
                {
                    throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
                }

                StackEntry op1;
                Operation op;
                if (opcode != Code.Brfalse && opcode != Code.Brtrue)
                {
                    op1 = _importer.PopExpression();
                    if (op2.Kind != StackValueKind.Int32)
                    {
                        throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
                    }
                    op = Operation.Eq + (opcode - Code.Beq);
                }
                else
                {
                    op1 = new Int32ConstantEntry((short)(opcode == Code.Brfalse ? 0 : 1));
                    op = Operation.Eq;
                }
                op1 = new BinaryOperator(op, op1, op2, StackValueKind.Int32);
                _importer.ImportAppendTree(new JumpTrueEntry(targetBlock.Label, op1));
            }
            else
            {
                _importer.ImportAppendTree(new JumpEntry(targetBlock.Label));
            }

            // Fall through handling
            _importer.ImportFallThrough(targetBlock);

            if (fallthroughBlock != null)
            {
                _importer.ImportFallThrough(fallthroughBlock);
            }

            context.StopImporting = true;
        }
    }
}
