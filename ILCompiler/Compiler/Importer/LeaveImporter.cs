using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LeaveImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode != OpCodes.Leave && instruction.OpCode != OpCodes.Leave_S) return false;

            var target = instruction.OperandAs<Instruction>();

            var targetBlock = importer.BasicBlocks[(int)target.Offset];

            importer.ImportAppendTree(new JumpEntry(targetBlock.Label));
            importer.ImportFallThrough(targetBlock);

            // TODO: Finally blocks

            context.StopImporting = true;

            return true;
        }
    }
}