using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode : DependencyNode
    {
        public MethodDesc Method { get; }
        public override string Name => Method.FullName;

        public Z80MethodCodeNode(MethodDesc method)
        {
            Method = method;
            ParamsCount = method.Parameters().Count;
            LocalsCount = method.Body?.Variables.Count ?? 0;
        }

        public string? MethodCode { get; set; }

        public int ParamsCount { get; set; }
        public int LocalsCount { get; set; }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context)
        {
            var scanner = new ILScanner(Method, context.NodeFactory, context.CorLibModuleProvider, context.PreinitializationManager);
            return scanner.FindDependencies();
        }
    }
}
