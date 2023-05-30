using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.IL;

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

                _dependencies.Add(_nodeFactory.TypeNode(fieldDef));
            }
        }

        private void ImportCall(Instruction instruction)
        {
            if (instruction.OpCode.Code == Code.Newobj)
            {
                CreateConstructedEETypeNodeDependencies(instruction);
            }

            var method = instruction.Operand as IMethod;
            if (method != null)
            {
                Z80MethodCodeNode methodNode;

                if (method.IsMethodSpec)
                {
                    methodNode = _nodeFactory.MethodNode((MethodSpec)method, _method);
                }
                else
                {
                    var methodDef = method.ResolveMethodDefThrow();
                    if (methodDef.HasCustomAttribute("System.Diagnostics.CodeAnalysis", "DynamicDependencyAttribute"))
                    {
                        // For dynamic dependencies we need to include the method referred to as part of the dependencies
                        // of the overall method being analysed
                        var dependentTypeAttribute = methodDef.CustomAttributes.Find("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");

                        var dependentMethodName = dependentTypeAttribute.ConstructorArguments[0].Value.ToString();
                        if (dependentMethodName == null) throw new InvalidOperationException("DynamicDependencyAttribute missing method name");

                        var dependentMethod = methodDef.DeclaringType.FindMethodEndsWith(dependentMethodName);
                        if (dependentMethod == null) throw new InvalidOperationException($"Cannot find dynamic dependency {dependentMethodName}");

                        method = dependentMethod;
                    }
                    methodNode = _nodeFactory.MethodNode(method);
                }

                _dependencies.Add(methodNode);
            }
        }

        private void CreateConstructedEETypeNodeDependencies(Instruction instruction)
        {
            if (instruction.Operand is not IMethodDefOrRef methodDefOrRef)
            {
                throw new InvalidOperationException("Newobj called with Operand which isn't a IMethodDefOrRef");
            }

            var declaringTypeSig = methodDefOrRef.DeclaringType.ToTypeSig();

            if (declaringTypeSig.IsArray)
            {
                // TODO: Will need to review this when changing NewArray assembly routine to take EEType instead of size
            }
            else
            {
                var methodToCall = methodDefOrRef.ResolveMethodDefThrow();
                var declType = methodToCall.DeclaringType;

                var objType = declType.ToTypeSig();

                if (!declType.IsValueType)
                {
                    // Determine required size on GC heap
                    var allocSize = objType.GetInstanceByteCount();

                    var constructedEETypeNode = new ConstructedEETypeNode(declType, allocSize);
                    _dependencies.Add(constructedEETypeNode);
                }
            }
        }
    }
}
