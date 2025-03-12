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

        private TypeDesc? _constrained;

        public ILScanner(MethodDesc method, MethodIL methodIL, DependencyNodeContext context, DnlibModule module)
        {
            _method = method;
            _methodIL = methodIL;
            _context = context;
            _module = module;
        }

        public IList<IDependencyNode> FindDependencies()
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

                        case ILOpcode.ldtoken:
                            ImportLoadToken(currentInstruction);
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

                        case ILOpcode.constrained:
                            ImportConstrainedPrefix(currentInstruction);
                            break;
                    }
                    currentOffset += currentInstruction.GetSize();
                    currentIndex++;
                }

                AddCatchTypeDependencies();
            }

            return _dependencies;
        }

        private void ImportConstrainedPrefix(Instruction instruction)
        {
            _constrained = (TypeDesc)instruction.Operand;
        }

        private void ImportBox(Instruction instruction)
        {
            var runtimeDeterminedType = (TypeDesc)instruction.Operand;

            if (runtimeDeterminedType.IsRuntimeDeterminedSubtype)
            {
                // Can't add dependency on type as will be decided at runtime
            }
            else
            {
                _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(runtimeDeterminedType));
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
                    _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(catchTypeDef));
                }
            }
        }

        private void ImportCasting(Instruction instruction)
        {
            var typeDesc = (TypeDesc)instruction.Operand;

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

            _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(systemStringType));

            _dependencies.Add(_context.NodeFactory.SerializedStringObject((string)instruction.Operand));
        }

        private void ImportFieldAccess(Instruction instruction, bool isStatic)
        {
            var fieldDesc = (FieldDesc)instruction.Operand;

            if (isStatic || fieldDesc.IsStatic)
            {
                if (isStatic && !fieldDesc.IsStatic)
                {
                    throw new InvalidProgramException();
                }

                var metadataType = fieldDesc.OwningType as MetadataType;
                _dependencies.Add(_context.NodeFactory.StaticsNode(metadataType!));

                if (!_context.PreinitializationManager.IsPreinitialized(fieldDesc.OwningType))
                {
                    AddStaticTypeConstructorDependency(fieldDesc.OwningType);
                }
            }
        }

        private void ImportLoadToken(Instruction instruction)
        {
            if (instruction.Operand is FieldDesc field)
            {
                _dependencies.Add(_context.NodeFactory.FieldRvaDataNode(field));
            }
            else
            {
                throw new NotImplementedException();
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
            var elemTypeDef = (TypeDesc)instruction.Operand;
            var arrayType = elemTypeDef.MakeArrayType();
            _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(arrayType));
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

            var methodDesc = (MethodDesc)instruction.Operand;

            if (methodDesc.IsIntrinsic && methodDesc.OwningType.Name == "EEType" && methodDesc.OwningType.Namespace == "Internal.Runtime" && methodDesc.Name == "Of")
            {
                // Need to add constructed dependency on T
                var instantiatedMethod = (InstantiatedMethod)methodDesc;

                var objType = instantiatedMethod.Instantiation[0];

                _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(objType));
            }
            if (methodDesc.IsIntrinsic && methodDesc.Name == "get_Chars")
            {
                _dependencies.Add(GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException"));
            }
            if (methodDesc.IsIntrinsic && methodDesc.OwningType.Name == "Console" && methodDesc.OwningType.Namespace == "System" && methodDesc.Name == "Write")
            {
                var parameter = methodDesc.Parameters[0];
                if (parameter.Type == methodDesc.Context.GetWellKnownType(WellKnownType.Char))
                {
                    // No need to add any dependencies as this gets inlined
                    return;
                }
            }
            if (methodDesc.IsIntrinsic && methodDesc.OwningType.Name == "RuntimeHelpers" && methodDesc.OwningType.Namespace == "System.Runtime.CompilerServices" && methodDesc.Name == "InitializeArray")
            {
                // This gets handled in codegen
                return;
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

            TypeDesc exactType = methodDesc.OwningType;

            bool resolvedConstraint = false;

            MethodDesc methodAfterConstraintResolution = methodDesc;
            if (_constrained != null)
            {
                TypeDesc constrained = _constrained;
                if (constrained.IsRuntimeDeterminedSubtype)
                    constrained = constrained.ConvertToCanonForm(TypeSystem.Canon.CanonicalFormKind.Specific);

                var constrainedType = constrained.Context.GetClosestDefType(constrained);
                MethodDesc? directMethod = constrainedType.TryResolveConstraintMethodApprox(methodDesc.OwningType, methodDesc);
                if (directMethod != null)
                {
                    methodAfterConstraintResolution = directMethod;
                    exactType = directMethod.OwningType;
                    resolvedConstraint = true;
                }
                else if (methodDesc.Signature.IsStatic)
                {
                    exactType = constrained;
                }
                else
                {
                    // Boxing will allocate new object of the constrained type
                    // so must include the constrained type as a constructed ee type
                    // in the dependency graph
                    _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(constrained));
                }

                _constrained = null;
            }

            MethodDesc targetMethod = methodAfterConstraintResolution;

            if (targetMethod.OwningType.IsInterface)
            {
                _dependencies.Add(_context.NodeFactory.NecessaryTypeSymbol(targetMethod.OwningType));
            }

            bool exactContextNeedsRuntimeLookup;
            if (targetMethod.HasInstantiation)
            {
                exactContextNeedsRuntimeLookup = targetMethod.IsSharedByGenericInstantiations;
            }
            else
            {
                exactContextNeedsRuntimeLookup = exactType.IsCanonicalSubtype(CanonicalFormKind.Any);
            }

            IMethodNode methodNode;
            bool directCall = IsDirectCall(targetMethod, instruction.Opcode, resolvedConstraint);
            if (directCall)
            {
                targetMethod = targetMethod.GetCanonMethodTarget(CanonicalFormKind.Specific);

                if (targetMethod.IsAbstract)
                {
                    throw new Exception("Bad IL cannot direct call abstract method");
                }

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
            else if (targetMethod.HasInstantiation)
            {
                // TODO: Generic Virtual Method Call
                throw new NotImplementedException("Generic Virtual Method Call");
            }
            else if (exactType.IsInterface)
            {
                if (exactContextNeedsRuntimeLookup)
                {
                    // TODO: Interface Method Call
                    throw new NotImplementedException();
                }
                else
                {
                    _dependencies.Add(_context.NodeFactory.VirtualMethodUse(targetMethod));
                    return;
                }
            }
            else
            {
                _dependencies.Add(_context.NodeFactory.VirtualMethodUse(targetMethod));
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

        private static bool IsDirectCall(MethodDesc targetMethod, ILOpcode opcode, bool resolvedConstraint)
        {
            bool directCall = false;

            if (targetMethod.IsStatic)
            {
                directCall = true;
            }
            else if (opcode != ILOpcode.callvirt || resolvedConstraint)
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
            var ctor = (MethodDesc)instruction.Operand;
            var owningType = ctor.OwningType;

            if (owningType.IsValueType)
            {
                // No dependency required 
            }
            else if (owningType.FullName != "System.String")
            {
                if (owningType.IsRuntimeDeterminedSubtype)
                {
                    // TODO: Handle runtime determined subtypes
                    throw new NotImplementedException();
                }
                else
                {
                    _dependencies.Add(_context.NodeFactory.ConstructedEETypeNode(owningType));
                }

                if (owningType.IsMdArray)
                {
                    // TODO: Add dependency for newing up multi-dimensional arrays
                    throw new NotImplementedException();
                }
                else
                {
                    // TODO: Add dependency on helper for NewObject
                }
            }
        }

        private IMethodNode GetHelperEntryPoint(string typeName, string methodName)
        {
            var helperMethod = _module.Context.GetHelperEntryPoint(typeName, methodName);
            return _context.NodeFactory.MethodNode(helperMethod);
        }
    }
}