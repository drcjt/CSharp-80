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
                else if (dependentMethod is ConstructedEETypeNode && !dependentMethod.Analysed)
                {
                    AnalyzeDependenciesForEETypeNode((ConstructedEETypeNode)dependentMethod);
                }

                codeNode.Dependencies.Add(dependentMethod);
            }
        }

        private void AnalyzeDependenciesForEETypeNode(ConstructedEETypeNode typeNode)
        {
            var baseType = typeNode.Type.BaseType;

            if (baseType != null)
            {
                var resolvedBaseType = baseType.ResolveTypeDefThrow();
                typeNode.RelatedType = resolvedBaseType;

                var objType = baseType.ToTypeSig();
                if (!objType.IsValueType)
                {
                    var allocSize = objType.GetInstanceByteCount();
                    var constructedEETypeNode = _nodeFactory.ConstructedEETypeNode(resolvedBaseType, allocSize);
                    typeNode.Dependencies.Add(constructedEETypeNode);

                    AnalyzeDependenciesForEETypeNode(constructedEETypeNode);
                }
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
