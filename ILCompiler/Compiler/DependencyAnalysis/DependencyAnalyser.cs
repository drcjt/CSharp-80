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
        private readonly CorLibModuleProvider _corLibModuleProvider;

        public DependencyAnalyser(MethodDesc method, NodeFactory nodeFactory, CorLibModuleProvider corLibModuleProvider)
        {
            _method = method;
            _nodeFactory = nodeFactory;
            _corLibModuleProvider = corLibModuleProvider;
        }

        public IList<IDependencyNode> FindDependencies()
        {
            if (!_method.IsIntrinsic)
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

                            case Code.Newarr:
                                ImportNewArray(currentInstruction);
                                break;

                            case Code.Ldstr:
                                ImportLoadString();
                                break;

                            case Code.Ldsfld:
                            case Code.Ldsflda:
                                ImportLoadField(currentInstruction, true);
                                break;

                            case Code.Stsfld:
                                ImportStoreField(currentInstruction, true);
                                break;

                            case Code.Isinst:
                                ImportCasting();
                                break;
                        }
                        currentOffset += currentInstruction.GetSize();
                        currentIndex++;
                    }
                }
            }

            return _dependencies;
        }

        private void ImportCasting()
        {
            var systemRuntimeTypeCast = _corLibModuleProvider.FindThrow("System.Runtime.TypeCast");
            var runtimeHelperMethod = systemRuntimeTypeCast.FindMethod("IsInstanceOfClass");

            var methodNode = _nodeFactory.MethodNode(runtimeHelperMethod);

            _dependencies.Add(methodNode);
        }

        private void ImportStoreField(Instruction instuction, bool isStatic)
        {
            ImportFieldAccess(instuction, isStatic);
        }

        private void ImportLoadField(Instruction instruction, bool isStatic)
        {
            ImportFieldAccess(instruction, isStatic);
        }

        private void ImportLoadString()
        {
            var systemStringType = _corLibModuleProvider.FindThrow("System.String");
            var objType = systemStringType.ToTypeSig();

            // Determine required size on GC heap
            var allocSize = objType.GetInstanceByteCount();

            _dependencies.Add(_nodeFactory.ConstructedEETypeNode(systemStringType, allocSize));
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

                _dependencies.Add(_nodeFactory.StaticsNode(fieldDef));
            }
        }

        private void ImportNewArray(Instruction instruction)
        {
            var elemTypeDef = (instruction.Operand as ITypeDefOrRef).ResolveTypeDefThrow();
            var allocSize = elemTypeDef.ToTypeSig().GetInstanceByteCount();

            var arrayType = elemTypeDef.MakeArrayType();

            _dependencies.Add(_nodeFactory.ConstructedEETypeNode(arrayType, allocSize));
        }

        private void ImportCall(Instruction instruction)
        {
            if (instruction.OpCode.Code == Code.Newobj)
            {
                CreateConstructedEETypeNodeDependencies(instruction);
            }

            if (instruction.Operand is IMethod method)
            {
                Z80MethodCodeNode methodNode;

                if (method.IsMethodSpec)
                {
                    var methodDef = method.ResolveMethodDefThrow();

                    if (methodDef.IsIntrinsic() && methodDef.DeclaringType.Name == "EETypePtr" && methodDef.DeclaringType.Namespace == "System" && methodDef.Name == "EETypePtrOf")
                    {
                        // Need to add constructed dependency on T
                        var genericParameters = ((MethodSpec)method).GenericInstMethodSig.GenericArguments;
                        var typeParam = genericParameters[0].TryGetTypeDefOrRef();
                        var typeDef = typeParam.ResolveTypeDef();

                        var objType = typeDef.ToTypeSig();

                        // Determine required size on GC heap
                        var allocSize = objType.GetInstanceByteCount();

                        _dependencies.Add(_nodeFactory.ConstructedEETypeNode(typeDef, allocSize));
                    }

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
                if (declaringTypeSig.FullName != "System.String")
                {
                    var objType = declaringTypeSig.ToClassOrValueTypeSig();
                    if (!objType.IsValueType)
                    {
                        // Determine required size on GC heap
                        var allocSize = objType.GetInstanceByteCount();

                        _dependencies.Add(_nodeFactory.ConstructedEETypeNode(objType.ToTypeDefOrRef(), allocSize));
                    }
                }
            }
        }
    }
}
