using dnlib.DotNet;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class TypeDependencyAnalyser
    {
        private readonly ILogger<TypeDependencyAnalyser> _logger;
        private readonly NodeFactory _nodeFactory;
        private readonly CorLibModuleProvider _corLibModuleProvider;

        public TypeDependencyAnalyser(ILogger<TypeDependencyAnalyser> logger, CorLibModuleProvider corLibModuleProvider)
        {
            _logger = logger;
            _nodeFactory = new NodeFactory();
            _corLibModuleProvider = corLibModuleProvider;
        }

        private void AnalyzeDependenciesForMethodCodeNode(Z80MethodCodeNode codeNode)
        {
            _logger.LogDebug("Analysing dependencies for {methodFullName}", codeNode.Method.FullName);
            codeNode.Analysed = true;

            var dependencyAnalyser = new DependencyAnalyser(codeNode.Method, _nodeFactory, _corLibModuleProvider);
            var dependencies = dependencyAnalyser.FindDependencies();

            foreach (var dependentMethod in dependencies)
            {
                if (dependentMethod is Z80MethodCodeNode && !dependentMethod.Analysed)
                {
                    AnalyzeDependenciesForMethodCodeNode((Z80MethodCodeNode)dependentMethod);
                }
                codeNode.Dependencies.Add(dependentMethod);
            }
        }

        public Z80MethodCodeNode AnalyseDependencies(MethodDef root)
        {
            var rootCodeNode = _nodeFactory.MethodNode(root);
            AnalyzeDependenciesForMethodCodeNode(rootCodeNode);

            return rootCodeNode;
        }
    }
}
