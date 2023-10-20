using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ConstructedEETypeNode : DependencyNode
    {
        public ITypeDefOrRef Type { get; private set; }
        public int BaseSize { get; private set; }

        public TypeDef? RelatedType { get; set; }

        public override string Name => Type.FullName + " constructed";

        private readonly INameMangler _nameMangler;
        private readonly PreinitializationManager _preinitializationManager;
        private readonly NodeFactory _nodeFactory;

        public ConstructedEETypeNode(ITypeDefOrRef type, int baseSize, INameMangler nameMangler, PreinitializationManager preinitializationManager, NodeFactory nodeFactory)
        {
            Type = type;
            BaseSize = baseSize;
            _nameMangler = nameMangler;
            _preinitializationManager = preinitializationManager;
            _nodeFactory = nodeFactory;
        }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context)
        {
            var dependencies = new List<IDependencyNode>();
            if (Type.ToTypeSig().IsSZArray)
            {
                var arrayType = context.CorLibModuleProvider.FindThrow("System.Array");

                var allocSize = arrayType.ToTypeSig().GetInstanceByteCount();
                var constructedEETypeNode = context.NodeFactory.ConstructedEETypeNode(arrayType, allocSize);
                dependencies.Add(constructedEETypeNode);
            }
            else
            {
                var baseType = Type.GetBaseType();
                if (baseType != null)
                {
                    var resolvedBaseType = baseType.ResolveTypeDefThrow();
                    RelatedType = resolvedBaseType;

                    var objType = baseType.ToTypeSig();
                    if (!objType.IsValueType)
                    {
                        var allocSize = objType.GetInstanceByteCount();
                        var constructedEETypeNode = context.NodeFactory.ConstructedEETypeNode(resolvedBaseType, allocSize);
                        dependencies.Add(constructedEETypeNode);
                    }
                }
            }

            return dependencies;
        }

        public override IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context)
        {
            var resolvedType = Type.ResolveTypeDefThrow();

            IList<ConditionalDependency> conditionalDependencies = new List<ConditionalDependency>();
            foreach (var method in VirtualMethodAlgorithm.EnumAllVirtualSlots(resolvedType))
            {
                if (!method.HasGenericParameters)
                {
                    var implementation = VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(resolvedType, method);

                    // Add a conditional dependency if the method implementation is on this type
                    // and is not abstract since there is no code for an abstract method
                    if (implementation?.DeclaringType == resolvedType && !implementation.IsAbstract)
                    {
                        var conditionalDependency = new ConditionalDependency
                        {
                            IfNode = context.NodeFactory.VirtualMethodUse(method),
                            ThenParent = this,
                            ThenNode = context.NodeFactory.MethodNode(implementation),
                        };
                        conditionalDependencies.Add(conditionalDependency);
                    }
                }
                else
                {
                    throw new NotImplementedException("Generic virtual methods not supported");
                }
            }

            var runtimeInterfaces = resolvedType.RuntimeInterfaces();
            for (int interfaceIndex = 0;  interfaceIndex < runtimeInterfaces.Length; interfaceIndex++) 
            {
                var interfaceType = runtimeInterfaces[interfaceIndex].ResolveTypeDefThrow();

                foreach (var method in interfaceType.Methods)
                {
                    if (method.IsVirtual)
                    {
                        var implMethod = VirtualMethodAlgorithm.ResolveInterfaceMethodToVirtualMethodOnType(method, resolvedType);
                        if (implMethod != null)
                        {
                            var conditionalDependency = new ConditionalDependency
                            {
                                IfNode = context.NodeFactory.VirtualMethodUse(method),
                                ThenParent = this,
                                ThenNode = context.NodeFactory.VirtualMethodUse(implMethod)
                            };
                            conditionalDependencies.Add(conditionalDependency);
                        }
                    }
                }
            }

            return conditionalDependencies;
        }
        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            var eeMangledTypeName = _nameMangler.GetMangledTypeName(Type);

            var instructionsBuilder = new InstructionsBuilder();
            instructionsBuilder.Comment($"{Type.FullName}");

            // Need to mangle full field name here
            instructionsBuilder.Label(eeMangledTypeName);

            // Emit data for EEType flags here
            ushort flags = 0;
            if (_preinitializationManager.HasLazyStaticConstructor(Type))
            {
                flags = 1;
            }
            instructionsBuilder.Dw(flags);

            // Emit data for EEType here
            var baseSize = BaseSize;

            byte lsb = (byte)(baseSize & 0xFF);
            byte msb = (byte)((baseSize >> 8) & 0xFF);

            instructionsBuilder.Db(lsb);
            instructionsBuilder.Db(msb);

            if (RelatedType != null)
            {
                var relatedTypeMangledTypeName = _nameMangler.GetMangledTypeName(RelatedType);
                instructionsBuilder.Dw(relatedTypeMangledTypeName);
            }
            else
            {
                instructionsBuilder.Dw((ushort)0);
            }

            // Emit VTable
            OutputVirtualSlots(instructionsBuilder, Type, Type);

            OutputDispatchMap(instructionsBuilder);

            return instructionsBuilder.Instructions;
        }

        private void OutputDispatchMap(InstructionsBuilder instructionsBuilder)
        {
            instructionsBuilder.Comment($"Dispatch map for {Type.FullName}");

            var resolvedType = Type.ResolveTypeDefThrow();
            var interfaces = Type.RuntimeInterfaces();

            // Enumerate each interface this type implements
            for (int interfaceIndex = 0; interfaceIndex < interfaces.Length; interfaceIndex++) 
            {
                var interfaceType = interfaces[interfaceIndex];
                var resolvedInterfaceType = interfaceType.ResolveTypeDefThrow();
                var virtualSlots = _nodeFactory.VTable(resolvedInterfaceType).GetSlots();

                // For each interface method slot try to emit a dispatch map
                for (int interfaceMethodSlot = 0; interfaceMethodSlot < virtualSlots.Count; interfaceMethodSlot++) 
                {
                    var method = virtualSlots[interfaceMethodSlot];

                    if (method.IsStatic)
                    {
                        // TODO: Static interface methods
                        throw new NotImplementedException("Static interface methods not currently supported");
                    }

                    var implMethod = VirtualMethodAlgorithm.ResolveInterfaceMethodToVirtualMethodOnType(method, resolvedType);
                    if (implMethod != null)
                    {
                        int emittedInterfaceSlot = interfaceMethodSlot;
                        int emittedImplSlot = VirtualMethodSlotHelper.GetVirtualMethodSlot(_nodeFactory, implMethod, resolvedType);

                        instructionsBuilder.Comment($"Interface {interfaceType.FullName}, {method.FullName}, {implMethod.FullName}");
                        instructionsBuilder.Dw((ushort)interfaceIndex);
                        instructionsBuilder.Dw((ushort)emittedInterfaceSlot);
                        instructionsBuilder.Dw((ushort)emittedImplSlot);
                    }
                    else
                    {
                        // No implementation in this type, so must be implemented by a base type in the hierarchy.
                        // No need to emit a dispatch map in this case as the runtime interface dispatch code will
                        // walk the inheritance chain

                        // TODO: Will need to check here for
                        //  * default implementation
                        //  * Reabstraction
                        //  * Diamond
                    }
                }
            }
        }

        private void OutputVirtualSlots(InstructionsBuilder instructionsBuilder, ITypeDefOrRef type, ITypeDefOrRef implType)
        {
            // Output inherited VTable slots first
            var baseType = type.GetBaseType();
            if (baseType != null)
            {
                OutputVirtualSlots(instructionsBuilder, baseType, implType);
            }

            // Now get new slots
            var resolvedType = type.ResolveTypeDefThrow();
            var vTable = _nodeFactory.VTable(resolvedType);
            var virtualSlots = vTable.GetSlots();

            // Emit VTable entries for the new slots
            for (int i = 0; i < virtualSlots.Count; i++)
            {
                var method = virtualSlots[i];
                var implementation = VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(implType.ResolveTypeDefThrow(), method);

                // Only generate slot entries for non abstract methods
                if (implementation != null && !implementation.IsAbstract)
                {
                    var implementationMangledName = _nameMangler.GetMangledMethodName(implementation);
                    instructionsBuilder.Dw(implementationMangledName);
                }
            }
        }
    }
}