﻿using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace ILCompiler.Compiler.DependencyAnalysisFramework
{
    public class DependencyAnalyzer
    {
        private readonly Stack<IDependencyNode> _markStack = new();
        private readonly List<IDependencyNode> _markedNodes = new();
        private readonly Dictionary<IDependencyNode, IList<ConditionalDependency>> _conditionalDependencyStore = new();

        private readonly NodeFactory _nodeFactory;
        private readonly DependencyNodeContext _nodeContext;

        public DependencyAnalyzer(ILogger<DependencyAnalyzer> logger, CorLibModuleProvider corLibModuleProvider, PreinitializationManager preinitializationManager, NodeFactory nodeFactory, IConfiguration configuration)
        {
            _nodeFactory = nodeFactory;
            _nodeContext = new DependencyNodeContext
            {
                Logger = logger,
                NodeFactory = _nodeFactory,
                CorLibModuleProvider = corLibModuleProvider,
                PreinitializationManager = preinitializationManager,
                Configuration = configuration,
            };
        }

        public IDependencyNode AddRoot(MethodDesc root)
        {
            var rootCodeNode = _nodeFactory.MethodNode(root);
            AddToMarkStack(rootCodeNode);
            return rootCodeNode;
        }

        public IImmutableList<IDependencyNode> ComputeMarkedNodes()
        {
            ProcessMarkStack();

            return _markedNodes.ToImmutableList<IDependencyNode>();
        }

        private void ProcessMarkStack()
        {
            while (_markStack.Count > 0)
            {
                var currentNode = _markStack.Pop();

                GetStaticDependencies(currentNode);

                if (_conditionalDependencyStore.TryGetValue(currentNode, out var conditionalDependencyList))
                {
                    foreach (var conditionalDependency in conditionalDependencyList)
                    {
                        conditionalDependency.ThenParent.Dependencies.Add(conditionalDependency.ThenNode);
                        AddToMarkStack(conditionalDependency.ThenNode);
                    }
                    _conditionalDependencyStore.Remove(currentNode);
                }
            }
        }

        private void GetStaticDependencies(IDependencyNode node)
        {
            var dependencies = node.GetStaticDependencies(_nodeContext);
            foreach (var dependency in dependencies)
            {
                AddToMarkStack(dependency);
            }

            foreach (var dependency in node.GetConditionalStaticDependencies(_nodeContext))
            {
                if (dependency.IfNode.Mark)
                {
                    node.Dependencies.Add(dependency.ThenNode);
                    AddToMarkStack(dependency.ThenNode);
                }
                else
                {
                    if (!_conditionalDependencyStore.TryGetValue(dependency.IfNode, out var conditionalDependencyList))
                    {
                        conditionalDependencyList = new List<ConditionalDependency>();
                        _conditionalDependencyStore.Add(dependency.IfNode, conditionalDependencyList);
                    }
                    conditionalDependencyList.Add(dependency);
                }
            }
            node.Dependencies = dependencies;
        }

        private void AddToMarkStack(IDependencyNode node)
        {
            if (!node.Mark)
            {
                node.Mark = true;
                _markedNodes.Add(node);

                node.OnMarked(_nodeFactory);

                _markStack.Push(node);
            }
        }
    }
}
