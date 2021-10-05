using dnlib.DotNet;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class TypeDependencyAnalyser
    {
        private readonly Compilation _compilation;

        public TypeDependencyAnalyser(Compilation compilation)
        {
            _compilation = compilation;
        }

        public Z80MethodCodeNode AnalyseDependencies(IList<TypeDef> types, MethodDef root)
        {
            var nodesByFullMethodName = new Dictionary<string, Z80MethodCodeNode>();
            var dependencies = new Dictionary<Z80MethodCodeNode, IList<MethodDef>>();
            foreach (var type in types)
            {
                _compilation.Logger.LogDebug("Analysing dependencies for Type {type.Name}", type.Name);

                foreach (var method in type.Methods)
                {
                    var methodCodeNode = new Z80MethodCodeNode(method);

                    if (!nodesByFullMethodName.ContainsKey(method.FullName))
                    {
                        nodesByFullMethodName.Add(method.FullName, methodCodeNode);

                        var dependencyAnalyser = new MethodDependencyAnalyser(method);
                        dependencies[methodCodeNode] = dependencyAnalyser.FindCallTargets();
                    }
                }
            }

            foreach (var dependency in dependencies)
            {
                var dependentNodes = new List<Z80MethodCodeNode>();
                foreach (var dependentMethodDef in dependency.Value)
                {
                    dependentNodes.Add(nodesByFullMethodName[dependentMethodDef.FullName]);
                }

                dependency.Key.Dependencies = dependentNodes;
            }

            return nodesByFullMethodName[root.FullName];
        }
    }
}
