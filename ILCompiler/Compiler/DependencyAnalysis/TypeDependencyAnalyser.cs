using dnlib.DotNet;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class TypeDependencyAnalyser
    {
        private readonly ILogger<TypeDependencyAnalyser> _logger;
        private readonly NodeFactory _nodeFactory;

        public TypeDependencyAnalyser(ILogger<TypeDependencyAnalyser> logger)
        {
            _logger = logger;
            _nodeFactory = new NodeFactory();
        }

        private void AnalyzeDependenciesForMethodCodeNode(Z80MethodCodeNode codeNode)
        {
            codeNode.Analysed = true;
            var dependencyAnalyser = new DependencyAnalyser(codeNode.Method, _nodeFactory);
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
