using dnlib.DotNet;
using ILCompiler.Compiler.PreInit;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class DependencyAnalyzer
    {
        private readonly Stack<IDependencyNode> _markStack = new();
        private readonly IList<IDependencyNode> _markedNodes = new List<IDependencyNode>();

        private readonly NodeFactory _nodeFactory;
        private readonly DependencyNodeContext _nodeContext;

        public IList<IDependencyNode> MarkedNodeList
        {
            get
            {
                return _markedNodes;
            }
        }

        public DependencyAnalyzer(ILogger<DependencyAnalyzer> logger, CorLibModuleProvider corLibModuleProvider, PreinitializationManager preinitializationManager)
        {
            _nodeFactory = new NodeFactory();
            _nodeContext = new DependencyNodeContext
            {
                Logger = logger,
                NodeFactory = _nodeFactory,
                CorLibModuleProvider = corLibModuleProvider,
                PreinitializationManager = preinitializationManager,
            };
        }

        public IDependencyNode AddRoot(MethodDef root)
        {
            var rootCodeNode = _nodeFactory.MethodNode(root);
            AddToMarkStack(rootCodeNode);
            return rootCodeNode;
        }

        public void ComputeMarkedNodes()
        {
            ProcessMarkStack();
        }

        private void ProcessMarkStack()
        {
            do
            {
                while (_markStack.Count > 0)
                {
                    var currentNode = _markStack.Pop();

                    GetStaticDependencies(currentNode);                    
                }
            } while (_markStack.Count != 0);
        }

        private void GetStaticDependencies(IDependencyNode node)
        {
            var dependencies = node.GetStaticDependencies(_nodeContext);
            foreach (var dependency in dependencies) 
            {
                AddToMarkStack(dependency);
            }
        }

        private void AddToMarkStack(IDependencyNode node) 
        {
            if (!node.Mark)
            {
                node.Mark = true;
                _markedNodes.Add(node);

                _markStack.Push(node);
            }
        }
    }
}
