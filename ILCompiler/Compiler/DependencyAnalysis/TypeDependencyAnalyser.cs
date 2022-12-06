using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class TypeDependencyAnalyser
    {
        private readonly ILogger<TypeDependencyAnalyser> _logger;

        public TypeDependencyAnalyser(ILogger<TypeDependencyAnalyser> logger)
        {
            _logger = logger;
        }

        public Z80MethodCodeNode AnalyseDependencies(IList<TypeDef> types, MethodDef root)
        {
            var nodesByFullMethodName = new Dictionary<string, Z80MethodCodeNode>();
            var dependencies = new Dictionary<Z80MethodCodeNode, IList<IMethod>>();
            foreach (var type in types)
            {
                _logger.LogDebug("Analysing dependencies for Type {type.Name}", type.Name);

                foreach (var method in type.Methods)
                {
                    // Only analyse non generic methods
                    if (!method.HasGenericParameters)
                    {
                        var methodDef = new MethodDesc(method);
                        var methodCodeNode = new Z80MethodCodeNode(methodDef);

                        if (!nodesByFullMethodName.ContainsKey(method.FullName))
                        {
                            nodesByFullMethodName.Add(method.FullName, methodCodeNode);

                            var dependencyAnalyser = new MethodDependencyAnalyser(method);
                            dependencies[methodCodeNode] = dependencyAnalyser.FindCallTargets();
                        }
                    }
                }
            }

            foreach (var dependency in dependencies)
            {
                var dependentNodes = new List<Z80MethodCodeNode>();
                foreach (var dependentMethod in dependency.Value)
                {
                    var dependentMethodFullName = dependentMethod.FullName;
                    if (dependentMethod.IsMethodSpec)
                    {
                        var methodSpec = (MethodSpec)dependentMethod;
                        if (!nodesByFullMethodName.ContainsKey(methodSpec.FullName))
                        {
                            IList<TypeSig> genericArguments = methodSpec.GenericInstMethodSig.GenericArguments;
                            var genericMethod = methodSpec.Method.ResolveMethodDef();
                            if (genericMethod != null)
                            {
                                var method = new InstantiatedMethod(genericMethod, genericArguments, methodSpec.FullName);
                                var instantiatedMethodCodeNode = new Z80MethodCodeNode(method);
                                nodesByFullMethodName[methodSpec.FullName] = instantiatedMethodCodeNode;
                            }
                        }

                        dependentMethodFullName = methodSpec.FullName;
                    }

                    var dependentNode = nodesByFullMethodName[dependentMethodFullName];
                    if (!dependentNodes.Contains(dependentNode))
                    {
                        dependentNodes.Add(dependentNode);
                    }
                }

                dependency.Key.Dependencies = dependentNodes;
            }

            return nodesByFullMethodName[root.FullName];
        }
    }
}
