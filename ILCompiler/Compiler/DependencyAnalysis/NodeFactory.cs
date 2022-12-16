using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class NodeFactory
    {
        private IDictionary<string, EEType> _typeNodesByFullName = new Dictionary<string, EEType>();
        private IDictionary<string, Z80MethodCodeNode> _methodNodesByFullName = new Dictionary<string, Z80MethodCodeNode>();
        
        public EEType TypeNode(TypeDef type)
        {
            if (!_typeNodesByFullName.TryGetValue(type.FullName, out var typeNode))
            {
                typeNode = new EEType(type);
                _typeNodesByFullName[type.FullName] = typeNode;
            }

            return typeNode;
        }

        public Z80MethodCodeNode MethodNode(IMethod method)
        {
            if (method.IsMethodSpec)
            {
                var methodSpec = (MethodSpec)method;
                IList<TypeSig> genericArguments = methodSpec.GenericInstMethodSig.GenericArguments;
                var methodDef = methodSpec.Method.ResolveMethodDefThrow();

                if (!_methodNodesByFullName.TryGetValue(methodSpec.FullName, out var methodNode))
                {
                    methodNode = new Z80MethodCodeNode(new InstantiatedMethod(methodDef, genericArguments, methodSpec.FullName));
                    _methodNodesByFullName[methodSpec.FullName] = methodNode;
                }

                return methodNode;
            }
            else
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
}
