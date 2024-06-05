using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class VirtualMethodUseNode : DependencyNode
    {
        public MethodDesc Method { get; }

        public override string Name => Method.FullName;

        public VirtualMethodUseNode(MethodDesc method)
        {       
            Method = method;
        }

        public override void OnMarked(NodeFactory factory)
        {
            // The virtual method is being used so record that a slot is required in the VTable
            var vTableSlice = factory.VTable(Method.OwningType);
            vTableSlice.AddEntry(Method);
        }
    }
}
