using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LeaveImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.leave && instruction.Opcode != ILOpcode.leave_s) return false;

            var target = (Instruction)instruction.GetOperand();

            var targetBlock = importer.BasicBlocks[(int)target.Offset];

            importer.ImportAppendTree(new JumpEntry(targetBlock.Label));
            importer.ImportFallThrough(targetBlock);

            // TODO: Finally blocks

            context.StopImporting = true;

            return true;
        }
    }
}