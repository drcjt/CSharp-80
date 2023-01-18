using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class DependencyAnalyser
    {
        private readonly MethodDesc _method;
        private readonly IList<IDependencyNode> _dependencies = new List<IDependencyNode>();
        private readonly NodeFactory _nodeFactory;

        public DependencyAnalyser(MethodDesc method, NodeFactory nodeFactory)
        {
            _method = method;
            _nodeFactory = nodeFactory;
        }

        public IList<IDependencyNode> FindDependencies()
        {
            var currentIndex = 0;
            var currentOffset = 0;

            if (_method.Body != null)
            {
                while (currentIndex < _method.Body.Instructions.Count)
                {
                    var currentInstruction = _method.Body.Instructions[currentIndex];

                    switch (currentInstruction.OpCode.Code)
                    {
                        case Code.Newobj:
                        case Code.Call:
                        case Code.Callvirt:
                            ImportCall(currentInstruction);
                            break;


                        case Code.Ldsfld:
                        case Code.Ldsflda:
                            ImportLoadField(currentInstruction, true);
                            break;

                        case Code.Stsfld:
                            ImportStoreField(currentInstruction, true);
                            break;
                            
                    }
                    currentOffset += currentInstruction.GetSize();
                    currentIndex++;
                }
            }

            return _dependencies;
        }

        private void ImportStoreField(Instruction instuction, bool isStatic)
        {
            ImportFieldAccess(instuction, isStatic);
        }

        private void ImportLoadField(Instruction instruction, bool isStatic)
        {
            ImportFieldAccess(instruction, isStatic);
        }

        private void ImportFieldAccess(Instruction instruction, bool isStatic)
        {
            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDefThrow();

            if (isStatic || fieldDef.IsStatic)
            {
                if (isStatic && !fieldDef.IsStatic)
                {
                    throw new InvalidProgramException();
                }

                if (fieldDef.HasFieldRVA)
                {
                    return;
                }

                var declaringType = fieldDef.DeclaringType;

                if (declaringType != null)
                {
                    _dependencies.Add(_nodeFactory.TypeNode(declaringType));
                }
            }
        }

        private void ImportCall(Instruction instruction)
        {
            var method = instruction.Operand as IMethod;
            if (method != null)
            {
                var methodNode = _nodeFactory.MethodNode(method);
                if (methodNode != null)
                {
                    _dependencies.Add(methodNode);
                }
            }
        }
    }
}
