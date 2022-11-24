using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class SwitchImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Switch) return false;

            var fallthroughBlock = context.FallThroughBlock;

            var op1 = importer.PopExpression();

            var targets = instruction.Operand as Instruction[];
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

            context.StopImporting = true;

            return true;
        }
    }
}
