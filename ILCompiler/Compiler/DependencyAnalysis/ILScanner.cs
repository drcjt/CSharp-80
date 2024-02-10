using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.PreInit;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ILScanner
    {
        private readonly MethodDesc _method;
        private readonly IList<IDependencyNode> _dependencies = new List<IDependencyNode>();
        private readonly NodeFactory _nodeFactory;
        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly PreinitializationManager _preinitializationManager;

        public ILScanner(MethodDesc method, NodeFactory nodeFactory, CorLibModuleProvider corLibModuleProvider, PreinitializationManager preinitializationManager)
        {
            _method = method;
            _nodeFactory = nodeFactory;
            _corLibModuleProvider = corLibModuleProvider;
            _preinitializationManager = preinitializationManager;
        }

        public IList<IDependencyNode> FindDependencies()
        {
            if (!_method.IsIntrinsic)
            {
                var currentIndex = 0;
                var currentOffset = 0;

                if (_method.Body != null)
                {
                    AddThrowExceptionIfAnyExceptionHandlers();

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
                                ImportLoadString(currentInstruction);
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

                            case Code.Ldelema:
                                ImportAddressOfElem();
                                break;

                            case Code.Ldelem:
                            case Code.Ldelem_I:
                            case Code.Ldelem_I1:
                            case Code.Ldelem_I2:
                            case Code.Ldelem_I4:
                            case Code.Ldelem_Ref:
                                ImportLoadElement();
                                break;

                            case Code.Stelem:
                            case Code.Stelem_I:
                            case Code.Stelem_I1:
                            case Code.Stelem_I2:
                            case Code.Stelem_I4:
                            case Code.Stelem_Ref:
                                ImportStoreElement();
                                break;
                        }
                        currentOffset += currentInstruction.GetSize();
                        currentIndex++;
                    }

                    AddCatchTypeDependencies();
                }
            }

            return _dependencies;
        }

        private void AddThrowExceptionIfAnyExceptionHandlers()
        {
            if (_method.HasExceptionHandlers)
            {
                var systemRuntimeExceptionHandling = _corLibModuleProvider.FindThrow("System.Runtime.ExceptionHandling");
                var runtimeHelperMethod = systemRuntimeExceptionHandling.FindMethod("ThrowException");
                var methodNode = _nodeFactory.MethodNode(runtimeHelperMethod);
                _dependencies.Add(methodNode);
            }
        }

        private void AddCatchTypeDependencies()
        {
            foreach (var exceptionHandler in _method.Body.ExceptionHandlers)
            {
                var catchTypeDef = exceptionHandler.CatchType.ResolveTypeDefThrow();
                var allocSize = catchTypeDef.ToTypeSig().GetInstanceByteCount();
                _dependencies.Add(_nodeFactory.ConstructedEETypeNode(catchTypeDef, allocSize));
            }
        }

        private void ImportCasting()
        {
            var systemRuntimeTypeCast = _corLibModuleProvider.FindThrow("System.Runtime.TypeCast");
            var runtimeHelperMethod = systemRuntimeTypeCast.FindMethod("IsInstanceOfClass");

            var methodNode = _nodeFactory.MethodNode(runtimeHelperMethod);

            _dependencies.Add(methodNode);
        }

        private void ImportAddressOfElem()
        {        
            _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException"));
        }

        private void ImportLoadElement()
        {
            _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException"));
        }

        private void ImportStoreElement()
        {
            _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException"));
        }

        private void AddThrowNullReferenceThrowHelperDependency()
        {
            _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowNullReferenceException"));
        }

        private void ImportStoreField(Instruction instuction, bool isStatic)
        {
            ImportFieldAccess(instuction, isStatic);
        }

        private void ImportLoadField(Instruction instruction, bool isStatic)
        {
            ImportFieldAccess(instruction, isStatic);
        }

        private void ImportLoadString(Instruction instruction)
        {
            var systemStringType = _corLibModuleProvider.FindThrow("System.String");
            var objType = systemStringType.ToTypeSig();

            // Determine required size on GC heap
            var allocSize = objType.GetInstanceByteCount();

            _dependencies.Add(_nodeFactory.ConstructedEETypeNode(systemStringType, allocSize));

            _dependencies.Add(_nodeFactory.SerializedStringObject(instruction.OperandAs<string>(), _corLibModuleProvider));
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

                if (!_preinitializationManager.IsPreinitialized(fieldDef.DeclaringType))
                {
                    AddStaticTypeConstructorDependency(fieldDef.DeclaringType);
                }
            }
        }

        private void AddStaticTypeConstructorDependency(TypeDef type)
        {
            var staticConstructoreMethod = type.FindStaticConstructor();
            if (staticConstructoreMethod != null)
            {
                var node = _nodeFactory.MethodNode(staticConstructoreMethod);

                // Ensure we don't have cyclic dependencies
                if (_method.FullName != staticConstructoreMethod.FullName)
                {
                    _dependencies.Add(node);
                }
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
            if (instruction.OpCode.Code == Code.Callvirt)
            {
                AddThrowNullReferenceThrowHelperDependency();
            }

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

                    if (methodDef.IsIntrinsic() && methodDef.DeclaringType.Name == "EEType" && methodDef.DeclaringType.Namespace == "Internal.Runtime" && methodDef.Name == "Of")
                    {
                        // Need to add constructed dependency on T
                        var genericParameters = ((MethodSpec)method).GenericInstMethodSig.GenericArguments;
                        var objType = GenericTypeInstantiator.Instantiate(genericParameters[0], method, _method);
                        var typeDef = objType.ToTypeDefOrRef().ResolveTypeDef();

                        // Determine required size on GC heap
                        var allocSize = objType.GetInstanceByteCount();

                        _dependencies.Add(_nodeFactory.ConstructedEETypeNode(typeDef, allocSize));
                    }

                    methodNode = _nodeFactory.MethodNode((MethodSpec)method, _method);
                }
                else
                {
                    var methodDef = method.ResolveMethodDefThrow();
                    if (methodDef.IsIntrinsic() && methodDef.Name == "get_Chars")
                    {
                        _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException"));
                    }

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

                    if (methodDef.DeclaringType.IsInterface)
                    {
                        _dependencies.Add(_nodeFactory.NecessaryTypeSymbol(methodDef.DeclaringType));
                    }

                    bool directCall = IsDirectCall(methodDef, instruction.OpCode.Code);
                    if (directCall)
                    {
                        methodNode = _nodeFactory.MethodNode(method);
                    }
                    else
                    {
                        _dependencies.Add(_nodeFactory.VirtualMethodUse(method));
                        return;
                    }
                }

                // Calling a static method on a class with a static constructor is a trigger for calling
                // the static constructor so add the static constructor as a dependency
                var declaringType = methodNode.Method.DeclaringType;
                if (methodNode.Method.IsStatic)
                {
                    var staticConstructorMethod = declaringType.FindStaticConstructor();
                    if (staticConstructorMethod != null && !_preinitializationManager.IsPreinitialized(declaringType))
                    {
                        AddStaticTypeConstructorDependency(methodNode.Method.DeclaringType);
                    }
                }
                else if (instruction.OpCode.Code == Code.Newobj && methodNode.Method.IsInstanceConstructor
                    && !_preinitializationManager.IsPreinitialized(declaringType))
                {
                    // Add dependency on static constructor if this is a NewObj for a type with a static constructor

                    AddStaticTypeConstructorDependency(methodNode.Method.DeclaringType);
                }

                _dependencies.Add(methodNode);
            }
        }

        private static bool IsDirectCall(MethodDef targetMethod, Code opcode)
        {
            bool directCall = false;

            if (targetMethod.IsStatic)
            {
                directCall = true;
            }
            else if (opcode != Code.Callvirt)
            {
                directCall = true;
            }
            else
            {
                if (!targetMethod.IsVirtual)
                {
                    directCall = true;
                }
            }
            return directCall;
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
            else if (declaringTypeSig.IsValueType)
            {
                // No dependency required 
            }
            else if (declaringTypeSig.FullName != "System.String")
            {
                var objType = declaringTypeSig.ToClassSig();
                // Determine required size on GC heap
                var allocSize = objType.GetInstanceByteCount();

                _dependencies.Add(_nodeFactory.ConstructedEETypeNode(objType.ToTypeDefOrRef(), allocSize));
            }
        }

        private Z80MethodCodeNode GetHelperEntryPoint(string typeName, string methodName)
        {
            var helperMethod = _corLibModuleProvider.GetHelperEntryPoint(typeName, methodName);
            return _nodeFactory.MethodNode(helperMethod);
        }
    }
}
