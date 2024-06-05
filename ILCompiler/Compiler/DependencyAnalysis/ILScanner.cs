using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.IL;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ILScanner
    {
        private readonly MethodDesc _method;
        private readonly IList<IDependencyNode> _dependencies = new List<IDependencyNode>();
        private readonly DependencyNodeContext _context;
        private readonly DnlibModule _module;

        public ILScanner(MethodDesc method, DependencyNodeContext context, DnlibModule module)
        {
            _method = method;
            _context = context;
            _module = module;
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
                                ImportCasting(currentInstruction);
                                break;

                            case Code.Ldelema:
                                ImportAddressOfElem();
                                break;

                            case Code.Ldelem:
                            case Code.Ldelem_I:
                            case Code.Ldelem_I1:
                            case Code.Ldelem_I2:
                            case Code.Ldelem_I4:
                            case Code.Ldelem_U1:
                            case Code.Ldelem_U2:
                            case Code.Ldelem_U4:
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
                var systemRuntimeExceptionHandling = _context.CorLibModuleProvider.FindThrow("System.Runtime.ExceptionHandling");
                var runtimeHelperMethod = systemRuntimeExceptionHandling.FindMethod("ThrowException");
                var methodNode = _context.NodeFactory.MethodNode(_module.Create(runtimeHelperMethod));
                _dependencies.Add(methodNode);
            }
        }

        private void AddCatchTypeDependencies()
        {
            foreach (var exceptionHandler in _method.Body.ExceptionHandlers)
            {
                var catchTypeDef = (DefType)_module.Create(exceptionHandler.CatchType);
                _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(catchTypeDef, catchTypeDef.InstanceByteCount.AsInt));
            }
        }

        private void ImportCasting(Instruction instruction)
        {
            var typeDesc = _module.Create((ITypeDefOrRef)instruction.Operand);

            if (typeDesc.IsArray)
            {
                throw new NotImplementedException();
            }
            else if (typeDesc.IsInterface)
            {
                throw new NotImplementedException();
            }
            else if (typeDesc.IsParameterizedType)
            {
                throw new NotImplementedException();
            }
            else
            {
                var systemRuntimeTypeCast = _context.CorLibModuleProvider.FindThrow("System.Runtime.TypeCast");
                var runtimeHelperMethod = systemRuntimeTypeCast.FindMethod("IsInstanceOfClass");
                var methodNode = _context.NodeFactory.MethodNode(_module.Create(runtimeHelperMethod));

                _dependencies.Add(methodNode);
                _dependencies.Add(_context.NodeFactory.NecessaryTypeSymbol(typeDesc));
            }
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
            if (!_context.Configuration.SkipNullReferenceCheck)
            {
                _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowNullReferenceException"));
            }
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
            var systemStringType = (DefType)_module.GetType("System", "String");

            _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(systemStringType, systemStringType.InstanceByteCount.AsInt));

            _dependencies.Add(_context.NodeFactory.SerializedStringObject(instruction.OperandAs<string>()));
        }

        private void ImportFieldAccess(Instruction instruction, bool isStatic)
        {
            var fieldDesc = _module.Create((IField)instruction.Operand);

            if (isStatic || fieldDesc.IsStatic)
            {
                if (isStatic && !fieldDesc.IsStatic)
                {
                    throw new InvalidProgramException();
                }

                _dependencies.Add(_context.NodeFactory.StaticsNode(fieldDesc));

                if (!_context.PreinitializationManager.IsPreinitialized(fieldDesc.OwningType))
                {
                    AddStaticTypeConstructorDependency(fieldDesc.OwningType);
                }
            }
        }

        private void AddStaticTypeConstructorDependency(TypeDesc type)
        {
            var staticConstructoreMethod = type.GetStaticConstructor();
            if (staticConstructoreMethod != null)
            {
                var node = _context.NodeFactory.MethodNode(staticConstructoreMethod);

                // Ensure we don't have cyclic dependencies
                if (_method.FullName != staticConstructoreMethod.FullName)
                {
                    _dependencies.Add(node);
                }
            }
        }

        private void ImportNewArray(Instruction instruction)
        {
            var elemTypeDef = _module.Create((ITypeDefOrRef)instruction.Operand, _method.Instantiation);
            var allocSize = elemTypeDef.GetElementSize().AsInt;

            var arrayType = new ArrayType(elemTypeDef, -1);

            _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(arrayType, allocSize));
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

                var methodDesc = _module.Create(method);

                // TODO: This can be removed when we instantiate the method IL.
                // For a case where we have a generic method passing on the generic parameters to another generic method
                // Then the call will instantiate the method signature - see code in InstantiatedMethodIL.GetObject
                if (_method is InstantiatedMethod)
                {
                    methodDesc = methodDesc.Context.GetInstantiatedMethod(methodDesc, _method.Instantiation);
                }

                methodNode = _context.NodeFactory.MethodNode(methodDesc);

                if (methodDesc.IsIntrinsic && methodDesc.OwningType.Name == "EEType" && methodDesc.OwningType.Namespace == "Internal.Runtime" && methodDesc.Name == "Of")
                {
                    // Need to add constructed dependency on T
                    var instantiatedMethod = (InstantiatedMethod)methodDesc;

                    var objType = instantiatedMethod.Instantiation[0];
                    var allocSize = objType.GetElementSize().AsInt;

                    _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(objType, allocSize));
                }
                if (methodDesc.IsIntrinsic && methodDesc.Name == "get_Chars")
                {
                    _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException"));
                }
                if (methodDesc.HasCustomAttribute("System.Diagnostics.CodeAnalysis", "DynamicDependencyAttribute"))
                {
                    // For dynamic dependencies we need to include the method referred to as part of the dependencies
                    // of the overall method being analysed
                    var dependentTypeAttribute = methodDesc.CustomAttributes.Find("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");

                    var dependentMethodName = dependentTypeAttribute.ConstructorArguments[0].Value.ToString();
                    if (dependentMethodName == null) throw new InvalidOperationException("DynamicDependencyAttribute missing method name");

                    var dependentMethod = methodDesc.OwningType.FindMethodEndsWith(dependentMethodName);
                    if (dependentMethod == null) throw new InvalidOperationException($"Cannot find dynamic dependency {dependentMethodName}");

                    methodDesc = dependentMethod;
                }

                if (methodDesc.OwningType.IsInterface)
                {
                    _dependencies.Add(_context.NodeFactory.NecessaryTypeSymbol(methodDesc.OwningType));
                }

                bool directCall = IsDirectCall(methodDesc, instruction.OpCode.Code);
                if (directCall)
                {
                    methodNode = _context.NodeFactory.MethodNode(methodDesc);
                }
                else
                {
                    _dependencies.Add(_context.NodeFactory.VirtualMethodUse(methodDesc));
                    return;
                }

                // Calling a static method on a class with a static constructor is a trigger for calling
                // the static constructor so add the static constructor as a dependency
                var owningType = methodNode.Method.OwningType;
                if (methodNode.Method.IsStatic)
                {
                    var staticConstructorMethod = owningType.GetStaticConstructor();
                    if (staticConstructorMethod != null && !_context.PreinitializationManager.IsPreinitialized(owningType))
                    {
                        AddStaticTypeConstructorDependency(owningType);
                    }
                }
                else if (instruction.OpCode.Code == Code.Newobj && methodNode.Method.IsDefaultConstructor
                    && !_context.PreinitializationManager.IsPreinitialized(owningType))
                {
                    // Add dependency on static constructor if this is a NewObj for a type with a static constructor

                    AddStaticTypeConstructorDependency(owningType);
                }

                _dependencies.Add(methodNode);
            }
        }

        private static bool IsDirectCall(MethodDesc targetMethod, Code opcode)
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

            var ctor = _module.Create(methodDefOrRef);
            var owningType = ctor.OwningType;

            if (owningType is ArrayType)
            {
                // TODO: Will need to review this when changing NewArray assembly routine to take EEType instead of size
                throw new NotImplementedException();
            }
            else if (owningType.IsValueType)
            {
                // No dependency required 
            }
            else if (owningType.FullName != "System.String")
            {
                var allocSize = owningType.GetElementSize().AsInt;
                _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(owningType, allocSize));
            }
        }

        private Z80MethodCodeNode GetHelperEntryPoint(string typeName, string methodName)
        {
            var helperMethod = _context.CorLibModuleProvider.GetHelperEntryPoint(typeName, methodName);
            return _context.NodeFactory.MethodNode(_module.Create(helperMethod));
        }
    }
}
