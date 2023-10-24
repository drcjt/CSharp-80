using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.IoC;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class NodeFactory
    {
        private readonly IDictionary<string, StaticsNode> _staticNodesByFullName = new Dictionary<string, StaticsNode>();
        private readonly IDictionary<string, Z80MethodCodeNode> _methodNodesByFullName = new Dictionary<string, Z80MethodCodeNode>();
        private readonly IDictionary<string, VirtualMethodUseNode> _virtualMethodNodesByFullName = new Dictionary<string, VirtualMethodUseNode>();
        private readonly IDictionary<string, ConstructedEETypeNode> _constructedEETypeNodesByFullName = new Dictionary<string, ConstructedEETypeNode>();
        private readonly IDictionary<TypeDef, VTableSliceNode> _vTableNodes = new Dictionary<TypeDef, VTableSliceNode>();
        private readonly IDictionary<ITypeDefOrRef, EETypeNode> _necessaryTypeSymbolNodes = new Dictionary<ITypeDefOrRef, EETypeNode>();

        private readonly PreinitializationManager _preinitializationManager;
        private readonly INameMangler _nameMangler;
        private readonly Factory<IMethodCompiler> _methodCompilerFactory;

        public NodeFactory(PreinitializationManager preinitializationManager, INameMangler nameMangler, Factory<IMethodCompiler> methodCompilerFactory) 
        {
            _preinitializationManager = preinitializationManager;
            _nameMangler = nameMangler;
            _methodCompilerFactory = methodCompilerFactory;
        }

        public ConstructedEETypeNode ConstructedEETypeNode(ITypeDefOrRef type, int size)
        {
            if (!_constructedEETypeNodesByFullName.TryGetValue(type.FullName, out var constructedEETypeNode))
            {
                constructedEETypeNode = new ConstructedEETypeNode(type, size, _nameMangler, _preinitializationManager, this);
                _constructedEETypeNodesByFullName[type.FullName] = constructedEETypeNode;
            }

            return constructedEETypeNode;
        }

        public EETypeNode NecessaryTypeSymbol(ITypeDefOrRef type)
        {
            if (!_necessaryTypeSymbolNodes.TryGetValue(type, out var necessaryTypeSymbolNode))
            {
                necessaryTypeSymbolNode = new EETypeNode(type, _nameMangler);
                _necessaryTypeSymbolNodes[type] = necessaryTypeSymbolNode;
            }

            return necessaryTypeSymbolNode;
        }

        public StaticsNode StaticsNode(FieldDef field)
        {
            if (!_staticNodesByFullName.TryGetValue(field.FullName, out var staticNode))
            {
                staticNode = new StaticsNode(field, _preinitializationManager, _nameMangler);
                _staticNodesByFullName[field.FullName] = staticNode;
            }

            return staticNode;
        }

        public VirtualMethodUseNode VirtualMethodUse(IMethod method)
        {
            var methodDef = method.ResolveMethodDefThrow();
            if (!_virtualMethodNodesByFullName.TryGetValue(methodDef.FullName, out var methodNode))
            {
                methodNode = new VirtualMethodUseNode(new MethodDesc(methodDef));
                _virtualMethodNodesByFullName[methodDef.FullName] = methodNode;
            }

            return methodNode;
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
                var instantiatedMethod = new InstantiatedMethod(calleeMethodDef, resolvedGenericParameters, calleeMethodFullName);
                methodNode = new Z80MethodCodeNode(instantiatedMethod, _methodCompilerFactory);
                _methodNodesByFullName[calleeMethodFullName] = methodNode;
            }

            return methodNode;
        }

        public Z80MethodCodeNode MethodNode(IMethod method)
        {
            var methodDef = method.ResolveMethodDefThrow();
            if (!_methodNodesByFullName.TryGetValue(methodDef.FullName, out var methodNode))
            {
                methodNode = new Z80MethodCodeNode(new MethodDesc(methodDef), _methodCompilerFactory);
                _methodNodesByFullName[methodDef.FullName] = methodNode;
            }

            return methodNode;
        }

        public VTableSliceNode VTable(TypeDef type)
        {
            if (!_vTableNodes.TryGetValue(type, out var vTableSliceNode))
            {
                vTableSliceNode = new VTableSliceNode(type);
                _vTableNodes[type] = vTableSliceNode;
            }

            return vTableSliceNode;
        }
    }
}
