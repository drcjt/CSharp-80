using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class SwitchImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.switch_) return false;

            var fallthroughBlock = importer.FallThroughBlock;

            var op1 = importer.Pop();

            var targets = (Instruction[])instruction.Operand;
            var jumpTable = new List<string>(targets?.Length ?? 0);
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    var targetBlock = importer.BasicBlocks[(int)target.Offset];
                    jumpTable.Add(targetBlock.Label);
                    importer.ImportFallThrough(targetBlock);
                }
            }

            var switchNode = new SwitchEntry(op1, jumpTable);

            importer.ImportAppendTree(switchNode);

            // TODO: Can this ever be null?
            if (fallthroughBlock != null)
            {
                importer.ImportFallThrough(fallthroughBlock);
            }

            importer.StopImporting = true;

            return true;
        }
    }
}