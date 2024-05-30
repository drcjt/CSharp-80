using ILCompiler.Common.TypeSystem.Common;
using System.Text;

namespace ILCompiler.Compiler
{
    internal static class Diagnostics
    {
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
