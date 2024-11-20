using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.IL;
using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ILScanner
    {
        private readonly MethodDesc _method;
        private MethodIL _methodIL;
        private readonly IList<IDependencyNode> _dependencies = new List<IDependencyNode>();
        private readonly DependencyNodeContext _context;
        private readonly DnlibModule _module;

        public ILScanner(MethodDesc method, MethodIL methodIL, DependencyNodeContext context, DnlibModule module)
        {
            _method = method;
            _methodIL = methodIL;
            _context = context;
            _module = module;
        }

        public IList<IDependencyNode> FindDependencies()
        {
            if (!_method.IsIntrinsic)
            {
                var currentIndex = 0;
                var currentOffset = 0;

                if (_methodIL != null)
                {
                    var uninstantiatedMethodIL = _methodIL.GetMethodILDefinition();
                    if (_methodIL != uninstantiatedMethodIL)
                    {
                        var sharedMethod = _method.GetSharedRuntimeFormMethodTarget();
                        _methodIL = new InstantiatedMethodIL(sharedMethod, uninstantiatedMethodIL);
                    }

                    AddThrowExceptionIfAnyExceptionHandlers();

                    while (currentIndex < _methodIL.Instructions.Count)
                    {
                        var currentInstruction = _methodIL.Instructions[currentIndex];

                        switch (currentInstruction.Opcode)
                        {
                            case ILOpcode.newobj:
                            case ILOpcode.call:
                            case ILOpcode.callvirt:
                                ImportCall(currentInstruction);
                                break;

                            case ILOpcode.newarr:
                                ImportNewArray(currentInstruction);
                                break;

                            case ILOpcode.ldstr:
                                ImportLoadString(currentInstruction);
                                break;

                            case ILOpcode.ldsfld:
                            case ILOpcode.ldsflda:
                                ImportLoadField(currentInstruction, true);
                                break;

                            case ILOpcode.stsfld:
                                ImportStoreField(currentInstruction, true);
                                break;

                            case ILOpcode.isinst:
                                ImportCasting(currentInstruction);
                                break;

                            case ILOpcode.ldelema:
                                ImportAddressOfElem();
                                break;

                            case ILOpcode.ldelem:
                            case ILOpcode.ldelem_i:
                            case ILOpcode.ldelem_i1:
                            case ILOpcode.ldelem_i2:
                            case ILOpcode.ldelem_i4:
                            case ILOpcode.ldelem_u1:
                            case ILOpcode.ldelem_u2:
                            case ILOpcode.ldelem_u4:
                            case ILOpcode.ldelem_ref:
                                ImportLoadElement();
                                break;

                            case ILOpcode.stelem:
                            case ILOpcode.stelem_i:
                            case ILOpcode.stelem_i1:
                            case ILOpcode.stelem_i2:
                            case ILOpcode.stelem_i4:
                            case ILOpcode.stelem_ref:
                                ImportStoreElement();
                                break;

                            case ILOpcode.box:
                                ImportBox(currentInstruction);
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

        private void ImportBox(Instruction instruction)
        {
            var runtimeDeterminedType = (TypeDesc)instruction.GetOperand();
            var allocSize = runtimeDeterminedType.GetElementSize().AsInt;

            if (runtimeDeterminedType.IsRuntimeDeterminedSubtype)
            {
                // Can't add dependency on type as will be decided at runtime
            }
            else
            {
                _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(runtimeDeterminedType, allocSize));
            }
        }

        private void AddThrowExceptionIfAnyExceptionHandlers()
        {
            if (_methodIL.GetExceptionRegions().Length > 0)
            {
                var systemRuntimeExceptionHandling = _context.CorLibModuleProvider.FindThrow("System.Runtime.ExceptionHandling");
                var runtimeHelperMethod = systemRuntimeExceptionHandling.FindMethod("ThrowException");
                var methodNode = _context.NodeFactory.MethodNode(_module.Create(runtimeHelperMethod));
                _dependencies.Add(methodNode);
            }
        }

        private void AddCatchTypeDependencies()
        {
            foreach (var exceptionHandler in _methodIL.GetExceptionRegions())
            {
                var catchTypeDef = (DefType)exceptionHandler.CatchType!;
                if (catchTypeDef != null)
                {
                    _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(catchTypeDef, catchTypeDef.InstanceByteCount.AsInt));
                }
            }
        }

        private void ImportCasting(Instruction instruction)
        {
            var typeDesc = (TypeDesc)instruction.GetOperand();

            string helperMethodName = "IsInstanceOfClass";
            if (typeDesc.IsArray)
            {
                throw new NotImplementedException();
            }
            else if (typeDesc.IsInterface)
            {
                helperMethodName = "IsInstanceOfInterface";
            }
            else if (typeDesc.IsParameterizedType)
            {
                throw new NotImplementedException();
            }

            var systemRuntimeTypeCast = _context.CorLibModuleProvider.FindThrow("System.Runtime.TypeCast");
            var runtimeHelperMethod = systemRuntimeTypeCast.FindMethod(helperMethodName);
            var methodNode = _context.NodeFactory.MethodNode(_module.Create(runtimeHelperMethod));

            _dependencies.Add(methodNode);
            _dependencies.Add(_context.NodeFactory.NecessaryTypeSymbol(typeDesc));
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

            _dependencies.Add(_context.NodeFactory.SerializedStringObject((string)instruction.GetOperand()));
        }

        private void ImportFieldAccess(Instruction instruction, bool isStatic)
        {
            var fieldDesc = (FieldDesc)instruction.GetOperand();

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
            var elemTypeDef = (TypeDesc)instruction.GetOperand();
            var allocSize = elemTypeDef.GetElementSize().AsInt;

            var arrayType = new ArrayType(elemTypeDef, -1);

            _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(arrayType, allocSize));
        }

        private void ImportCall(Instruction instruction)
        {
            if (instruction.Opcode == ILOpcode.callvirt)
            {
                AddThrowNullReferenceThrowHelperDependency();
            }

            if (instruction.Opcode == ILOpcode.newobj)
            {
                CreateConstructedEETypeNodeDependencies(instruction);
            }

            var methodDesc = (MethodDesc)instruction.GetOperand();

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
                var dependentMethodName = methodDesc.GetCustomAttributeValue("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");
                if (dependentMethodName == null) throw new InvalidOperationException("DynamicDependencyAttribute missing method name");

                var dependentMethod = methodDesc.OwningType.FindMethodEndsWith(dependentMethodName);
                if (dependentMethod == null) throw new InvalidOperationException($"Cannot find dynamic dependency {dependentMethodName}");

                methodDesc = dependentMethod;
            }

            if (methodDesc.OwningType.IsInterface)
            {
                _dependencies.Add(_context.NodeFactory.NecessaryTypeSymbol(methodDesc.OwningType));
            }

            bool exactContextNeedsRuntimeLookup;
            if (methodDesc.HasInstantiation)
            {
                exactContextNeedsRuntimeLookup = methodDesc.IsSharedByGenericInstantiations;
            }
            else
            {
                exactContextNeedsRuntimeLookup = methodDesc.OwningType.IsCanonicalSubtype(CanonicalFormKind.Any);
            }

            IMethodNode methodNode;
            bool directCall = IsDirectCall(methodDesc, instruction.Opcode);
            if (directCall)
            {
                var targetMethod = methodDesc.GetCanonMethodTarget(CanonicalFormKind.Specific);
                if (exactContextNeedsRuntimeLookup)
                {
                    if (targetMethod.RequiresInstMethodDescArg)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        methodNode = _context.NodeFactory.MethodNode(targetMethod);
                    }
                }
                else
                {
                    if (targetMethod.RequiresInstMethodDescArg)
                    {
                        // TODO: Get the Inst Param and add a dependency to it
                    }

                    methodNode = _context.NodeFactory.MethodNode(targetMethod);
                }
            }
            else if (methodDesc.HasInstantiation)
            {
                // TODO: Generic Virtual Method Call
                throw new NotImplementedException("Generic Virtual Method Call");
            }
            else if (methodDesc.OwningType.IsInterface)
            {
                if (exactContextNeedsRuntimeLookup)
                {
                    // TODO: Interface Method Call
                    throw new NotImplementedException();
                }
                else
                {
                    _dependencies.Add(_context.NodeFactory.VirtualMethodUse(methodDesc));
                    return;
                }
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
            else if (instruction.Opcode == ILOpcode.newobj && methodNode.Method.IsDefaultConstructor
                && !_context.PreinitializationManager.IsPreinitialized(owningType))
            {
                // Add dependency on static constructor if this is a NewObj for a type with a static constructor

                AddStaticTypeConstructorDependency(owningType);
            }

            _dependencies.Add(methodNode);
        }

        private static bool IsDirectCall(MethodDesc targetMethod, ILOpcode opcode)
        {
            bool directCall = false;

            if (targetMethod.IsStatic)
            {
                directCall = true;
            }
            else if (opcode != ILOpcode.callvirt)
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
            var ctor = (MethodDesc)instruction.GetOperand();
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

        private IMethodNode GetHelperEntryPoint(string typeName, string methodName)
        {
            var helperMethod = _module.Context.GetHelperEntryPoint(typeName, methodName);
            return _context.NodeFactory.MethodNode(helperMethod);
        }
    }
}