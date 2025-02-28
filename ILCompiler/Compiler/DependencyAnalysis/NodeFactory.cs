using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.IoC;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class NodeFactory
    {
        private readonly Dictionary<TypeDesc, StaticsNode> _staticNodes = [];
        private readonly Dictionary<MethodDesc, Z80MethodCodeNode> _methodNodesByFullName = [];
        private readonly Dictionary<string, UnboxingStubNode> _unboxingStubsByFullName = [];

        private readonly Dictionary<string, VirtualMethodUseNode> _virtualMethodNodesByFullName = [];
        private readonly Dictionary<string, ConstructedEETypeNode> _constructedEETypeNodesByFullName = [];
        private readonly Dictionary<TypeDesc, VTableSliceNode> _vTableNodes = [];
        private readonly Dictionary<TypeDesc, EETypeNode> _necessaryTypeSymbolNodes = [];
        private readonly Dictionary<string, FrozenStringNode> _frozenStringNodes = [];
        private readonly Dictionary<FieldDesc, FieldRvaDataNode> _fieldRvaDataNodes = [];

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

        public bool ConstructedEETypeNodeDefined(TypeDesc type) => _constructedEETypeNodesByFullName.ContainsKey(type.FullName);

        public ConstructedEETypeNode ConstructedEETypeNode(TypeDesc type)
        {
            if (!_constructedEETypeNodesByFullName.TryGetValue(type.FullName, out var constructedEETypeNode))
            {
                constructedEETypeNode = new ConstructedEETypeNode(type, _nameMangler, _preinitializationManager, this);
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

        public StaticsNode StaticsNode(MetadataType type)
        {
            if (!_staticNodes.TryGetValue(type, out var staticNode))
            {
                staticNode = new StaticsNode(type, _preinitializationManager, _nameMangler);
                _staticNodes[type] = staticNode;
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

        public FieldRvaDataNode FieldRvaDataNode(FieldDesc field)
        {
            if (!_fieldRvaDataNodes.TryGetValue(field, out var fieldRvaDataNode))
            {
                fieldRvaDataNode = new FieldRvaDataNode(((DnlibField)field).GetFieldRvaData());
                _fieldRvaDataNodes[field] = fieldRvaDataNode;
            }

            return fieldRvaDataNode;
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
            if (!_methodNodesByFullName.TryGetValue(method, out var methodNode))
            {
                methodNode = new Z80MethodCodeNode(method, _methodCompilerFactory, _module);
                _methodNodesByFullName[method] = methodNode;
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
