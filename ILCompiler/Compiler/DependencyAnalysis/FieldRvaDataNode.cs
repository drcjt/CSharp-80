using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class FieldRvaDataNode : DependencyNode
    {
        private readonly byte[] _bytes;
        public readonly string Label;

        public FieldRvaDataNode(byte[] bytes)
        {
            _bytes = bytes;
            Label = LabelGenerator.GetLabel(LabelType.FieldRVAData);
        }

        public override string Name => "Field RVA Data Node";

        public override IList<Instruction> GetInstructions(string inputFilePath, IList<string> modules)
        {
            var instructionsBuilder = new InstructionsBuilder();

            instructionsBuilder.Label(Label);

            foreach (var @byte in _bytes)
            {
                instructionsBuilder.Db(@byte);
            }

            return instructionsBuilder.Instructions;
        }

    }
}
