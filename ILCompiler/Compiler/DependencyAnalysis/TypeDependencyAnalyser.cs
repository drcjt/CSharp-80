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

        public Z80MethodCodeNode GetDependenciesForMethod(IMethod method)
        {
            _logger.LogDebug($"Analysing dependencies for Method {method.Name}");

            Z80MethodCodeNode codeNode;
            MethodDef methodDef;
            bool dependenciesAlreadyAnalysed = true;
            if (method.IsMethodDef)
            {
                methodDef = method.ResolveMethodDefThrow();

                if (!_codeNodesByFullMethodName.ContainsKey(methodDef.FullName))
                {
                    codeNode = new Z80MethodCodeNode(new MethodDesc(methodDef));
                    _codeNodesByFullMethodName[methodDef.FullName] = codeNode;
                    dependenciesAlreadyAnalysed = false;
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
                    dependenciesAlreadyAnalysed = false;
                }
                codeNode = _codeNodesByFullMethodName[methodSpec.FullName];
            }
            else
            {
                throw new NotImplementedException();
            }

            if (!dependenciesAlreadyAnalysed)
            {
                AnalyzeDependenciesForMethodCodeNode(codeNode);
            }

            return codeNode;
        }

        private void AnalyzeDependenciesForMethodCodeNode(Z80MethodCodeNode codeNode)
        {
            codeNode.Dependencies = new List<Z80MethodCodeNode>();

            var dependencyAnalyser = new MethodDependencyAnalyser(codeNode.Method);
            var dependencies = dependencyAnalyser.FindCallTargets();

            foreach (var dependentMethod in dependencies)
            {
                if (!_codeNodesByFullMethodName.TryGetValue(dependentMethod.FullName, out var dependenciesForMethod))
                {
                    dependenciesForMethod = GetDependenciesForMethod(dependentMethod);
                }
                codeNode.Dependencies.Add(dependenciesForMethod);
            }
        }

        public Z80MethodCodeNode AnalyseDependencies(MethodDef root)
        {
            return GetDependenciesForMethod(root);
        }
    }
}
