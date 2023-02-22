using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class NodeFactory
    {
        private IDictionary<string, EETypeNode> _typeNodesByFullName = new Dictionary<string, EETypeNode>();
        private IDictionary<string, Z80MethodCodeNode> _methodNodesByFullName = new Dictionary<string, Z80MethodCodeNode>();
        
        public EETypeNode TypeNode(TypeDef type)
        {
            if (!_typeNodesByFullName.TryGetValue(type.FullName, out var typeNode))
            {
                typeNode = new EETypeNode(type);
                _typeNodesByFullName[type.FullName] = typeNode;
            }

            return typeNode;
        }

        public Z80MethodCodeNode MethodNode(MethodSpec calleeMethod, MethodDesc callerMethod)
        {
            var calleeMethodDef = calleeMethod.Method.ResolveMethodDefThrow();

            IList<TypeSig> callerMethodGenericParameters = new List<TypeSig>();
            if (callerMethod is InstantiatedMethod method)
            {
                callerMethodGenericParameters = method.GenericParameters;
            }

            var resolvedGenericParameters = new List<TypeSig>();
            foreach (var genericParameter in ((MethodSpec)calleeMethod).GenericInstMethodSig.GenericArguments)
            {
                resolvedGenericParameters.Add(GenericTypeInstantiator.Instantiate(genericParameter, callerMethodGenericParameters));
            }

            var calleeMethodFullName = FullNameFactory.MethodFullName(calleeMethodDef.DeclaringType?.FullName, calleeMethodDef.Name, calleeMethodDef.MethodSig, null, resolvedGenericParameters);

            if (!_methodNodesByFullName.TryGetValue(calleeMethodFullName, out var methodNode))
            {
                methodNode = new Z80MethodCodeNode(new InstantiatedMethod(calleeMethodDef, resolvedGenericParameters, calleeMethodFullName));
                _methodNodesByFullName[calleeMethodFullName] = methodNode;
            }

            return methodNode;
        }

        public Z80MethodCodeNode MethodNode(IMethod method)
        {
            var methodDef = method.ResolveMethodDefThrow();
            if (!_methodNodesByFullName.TryGetValue(methodDef.FullName, out var methodNode))
            {
                methodNode = new Z80MethodCodeNode(new MethodDesc(methodDef));
                _methodNodesByFullName[methodDef.FullName] = methodNode;
            }

            return methodNode;
        }
    }
}
