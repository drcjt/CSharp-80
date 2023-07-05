using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class NodeFactory
    {
        private readonly IDictionary<string, StaticsNode> _staticNodesByFullName = new Dictionary<string, StaticsNode>();
        private readonly IDictionary<string, Z80MethodCodeNode> _methodNodesByFullName = new Dictionary<string, Z80MethodCodeNode>();
        private readonly IDictionary<string, ConstructedEETypeNode> _constructedEETypeNodesByFullName = new Dictionary<string, ConstructedEETypeNode>();

        public ConstructedEETypeNode ConstructedEETypeNode(ITypeDefOrRef type, int size)
        {
            if (!_constructedEETypeNodesByFullName.TryGetValue(type.FullName, out var constructedEETypeNode))
            {
                constructedEETypeNode = new ConstructedEETypeNode(type, size);
                _constructedEETypeNodesByFullName[type.FullName] = constructedEETypeNode;
            }

            return constructedEETypeNode;
        }

        public StaticsNode StaticsNode(FieldDef field)
        {
            if (!_staticNodesByFullName.TryGetValue(field.FullName, out var staticNode))
            {
                staticNode = new StaticsNode(field);
                _staticNodesByFullName[field.FullName] = staticNode;
            }

            return staticNode;
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
            foreach (var genericParameter in calleeMethod.GenericInstMethodSig.GenericArguments)
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
