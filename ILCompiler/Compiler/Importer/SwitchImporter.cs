using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System.Collections.Generic;

namespace ILCompiler.Compiler.Importer
{
    public class SwitchImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Switch;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var fallthroughBlock = context.CurrentBlock;

            var op1 = importer.PopExpression();

            var targets = instruction.Operand as Instruction[];

            var jumpTable = new List<string>(targets.Length);
            foreach (var target in targets)
            {
                var targetBlock = importer.BasicBlocks[(int)target.Offset];
                jumpTable.Add(targetBlock.Label);
                importer.ImportFallThrough(targetBlock);
            }

            var switchNode = new SwitchEntry(op1, jumpTable);

            importer.ImportAppendTree(switchNode);

            // TODO: Can this ever be null?
            if (fallthroughBlock != null)
            {
                importer.ImportFallThrough(fallthroughBlock);
            }

            context.StopImporting = true;
        }
    }
}
