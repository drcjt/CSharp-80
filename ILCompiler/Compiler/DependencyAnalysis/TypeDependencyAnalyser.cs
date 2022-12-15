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

        private IDictionary<string, Z80MethodCodeNode> _codeNodesByFullMethodName = new Dictionary<string, Z80MethodCodeNode>();

        public Z80MethodCodeNode GetRootDependencyNode(IMethod method)
        {
            Z80MethodCodeNode codeNode;
            MethodDef methodDef;
            if (method.IsMethodDef)
            {
                methodDef = method.ResolveMethodDefThrow();

                if (!_codeNodesByFullMethodName.ContainsKey(methodDef.FullName))
                {
                    codeNode = new Z80MethodCodeNode(new MethodDesc(methodDef));
                    _codeNodesByFullMethodName[methodDef.FullName] = codeNode;
                }
                codeNode = _codeNodesByFullMethodName[methodDef.FullName];
            }
            else if (method.IsMethodSpec)
            {
                var methodSpec = (MethodSpec)method;
                IList<TypeSig> genericArguments = methodSpec.GenericInstMethodSig.GenericArguments;
                methodDef = methodSpec.Method.ResolveMethodDefThrow();

                if (!_codeNodesByFullMethodName.ContainsKey(methodSpec.FullName))
                {
                    var instantiatedMethodCodeNode = new Z80MethodCodeNode(new InstantiatedMethod(methodDef, genericArguments, methodSpec.FullName));
                    _codeNodesByFullMethodName[methodSpec.FullName] = instantiatedMethodCodeNode;
                }
                codeNode = _codeNodesByFullMethodName[methodSpec.FullName];
            }
            else
            {
                throw new NotImplementedException();
            }
            return codeNode;
        }

        private void AnalyzeDependenciesForMethodCodeNode(Z80MethodCodeNode codeNode)
        {
            codeNode.Analysed = true;
            var dependencyAnalyser = new DependencyAnalyser(codeNode.Method, _codeNodesByFullMethodName);
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
            var rootCodeNode = GetRootDependencyNode(root);
            AnalyzeDependenciesForMethodCodeNode(rootCodeNode);
            return rootCodeNode;
        }
    }
}
