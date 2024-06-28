using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.IoC;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class NodeFactory
    {
        private readonly IDictionary<string, StaticsNode> _staticNodesByFullName = new Dictionary<string, StaticsNode>();
        private readonly IDictionary<string, Z80MethodCodeNode> _methodNodesByFullName = new Dictionary<string, Z80MethodCodeNode>();
        private readonly IDictionary<string, UnboxingStubNode> _unboxingStubsByFullName = new Dictionary<string, UnboxingStubNode>();

        private readonly IDictionary<string, VirtualMethodUseNode> _virtualMethodNodesByFullName = new Dictionary<string, VirtualMethodUseNode>();
        private readonly IDictionary<string, ConstructedEETypeNode> _constructedEETypeNodesByFullName = new Dictionary<string, ConstructedEETypeNode>();
        private readonly IDictionary<TypeDesc, VTableSliceNode> _vTableNodes = new Dictionary<TypeDesc, VTableSliceNode>();
        private readonly IDictionary<TypeDesc, EETypeNode> _necessaryTypeSymbolNodes = new Dictionary<TypeDesc, EETypeNode>();
        private readonly IDictionary<string, FrozenStringNode> _frozenStringNodes = new Dictionary<string, FrozenStringNode>();

        private readonly PreinitializationManager _preinitializationManager;
        private readonly INameMangler _nameMangler;
        private readonly Factory<IMethodCompiler> _methodCompilerFactory;
        private readonly DnlibModule _module;

        public NodeFactory(PreinitializationManager preinitializationManager, INameMangler nameMangler, Factory<IMethodCompiler> methodCompilerFactory, DnlibModule module) 
        {
            _preinitializationManager = preinitializationManager;
            _nameMangler = nameMangler;
            _methodCompilerFactory = methodCompilerFactory;
            _module = module;
        }

        public ConstructedEETypeNode ConstructedEETypeNode(TypeDesc type, int size)
        {
            if (!_constructedEETypeNodesByFullName.TryGetValue(type.FullName, out var constructedEETypeNode))
            {
                constructedEETypeNode = new ConstructedEETypeNode(type, size, _nameMangler, _preinitializationManager, this, _module);
                _constructedEETypeNodesByFullName[type.FullName] = constructedEETypeNode;
            }

            return constructedEETypeNode;
        }

        public EETypeNode NecessaryTypeSymbol(TypeDesc type)
        {
            if (!_necessaryTypeSymbolNodes.TryGetValue(type, out var necessaryTypeSymbolNode))
            {
                necessaryTypeSymbolNode = new EETypeNode(type, _nameMangler);
                _necessaryTypeSymbolNodes[type] = necessaryTypeSymbolNode;
            }

            return necessaryTypeSymbolNode;
        }

        public StaticsNode StaticsNode(FieldDesc field)
        {
            if (!_staticNodesByFullName.TryGetValue(field.ToString(), out var staticNode))
            {
                staticNode = new StaticsNode(field, _preinitializationManager, _nameMangler);
                _staticNodesByFullName[field.ToString()] = staticNode;
            }

            return staticNode;
        }

        public FrozenStringNode SerializedStringObject(string data)
        {
            if (!_frozenStringNodes.TryGetValue(data, out var frozenStringNode))
            {
                frozenStringNode = new FrozenStringNode(data, _nameMangler, _module);
                _frozenStringNodes[data] = frozenStringNode;
            }

            return frozenStringNode;
        }

        public VirtualMethodUseNode VirtualMethodUse(MethodDesc method)
        {
            if (!_virtualMethodNodesByFullName.TryGetValue(method.FullName, out var methodNode))
            {
                methodNode = new VirtualMethodUseNode(method);
                _virtualMethodNodesByFullName[method.FullName] = methodNode;
            }

            return methodNode;
        }

        public IMethodNode MethodNode(MethodDesc method, bool unboxingStub = false)
        {
            if (!_methodNodesByFullName.TryGetValue(method.FullName, out var methodNode))
            {
                methodNode = new Z80MethodCodeNode(method, _methodCompilerFactory, _module);
                _methodNodesByFullName[method.FullName] = methodNode;
            }

            if (unboxingStub)
            {
                if (!_unboxingStubsByFullName.TryGetValue(method.FullName, out var unboxingStubNode))
                {
                    unboxingStubNode = new UnboxingStubNode(method, _nameMangler);
                    _unboxingStubsByFullName[method.FullName] = unboxingStubNode;
                }

                return unboxingStubNode;
            }

            return methodNode;
        }

        public VTableSliceNode VTable(TypeDesc type)
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
