using ILCompiler.TypeSystem.Common;
using System.Text;

namespace ILCompiler.Compiler
{
    internal static class Diagnostics
    {
        public static string DumpBasicBlocks(IList<BasicBlock> blocks)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--------------------------------------------------------------");
            sb.AppendLine("BBnum preds                [IL range]   ");
            sb.AppendLine("--------------------------------------------------------------");

            for (int blockNum = 1; blockNum <= blocks.Count; blockNum++)
            {
                var block = blocks[blockNum - 1];

                var predecessors = "";
                foreach (var predecessor in block.Predecessors)
                {
                    if (!string.IsNullOrEmpty(predecessors))
                    {
                        predecessors += ",";
                    }
                    var predecessorNum = blocks.IndexOf(predecessor) + 1;
                    predecessors += $"BB{predecessorNum:D2}";
                }

                predecessors = predecessors.PadRight(20, ' ');

                var ilRange = $"[{block.StartOffset:D3}..{block.EndOffset:D3})";

                sb.AppendLine($"BB{blockNum:D2}  {predecessors} {ilRange}");
            }

            sb.AppendLine("--------------------------------------------------------------");

            return sb.ToString();
        }

        public static void DumpFlowGraph(string inputFilePath, MethodDesc method, IList<BasicBlock> blocks)
        {
            var inputFolder = Path.GetDirectoryName(inputFilePath) ?? throw new ArgumentException("inputFilePath is null");
            var flowGraphFolder = Path.Combine(inputFolder, "FlowGraph");
            Directory.CreateDirectory(flowGraphFolder);

            var dotFilePath = Path.Combine(flowGraphFolder, $"{method.OwningType.Name}-{method.Name}.dot");

            using (StreamWriter dotOutput = new StreamWriter(dotFilePath, append: false))
            {
                dotOutput.WriteLine("digraph Flowgraph");
                dotOutput.WriteLine("{");
                dotOutput.WriteLine($"    graph [label=\"{method.FullName}\"];");
                dotOutput.WriteLine( "    node [shape=\"Box\"];");

                foreach (var block in blocks)
                {
                    dotOutput.WriteLine($"    {block.Label}");
                }

                foreach (var block in blocks)
                {
                    foreach (var successor in block.Successors)
                    {
                        dotOutput.WriteLine($"    {block.Label} -> {successor.Label}");
                    }
                }

                dotOutput.WriteLine("}");
            }
        }
    }
}
