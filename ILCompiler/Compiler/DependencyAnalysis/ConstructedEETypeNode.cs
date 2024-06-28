using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ConstructedEETypeNode : EETypeNode
    {
        public int BaseSize { get; private set; }

        public TypeDesc? RelatedType { get; set; }

        public override string Name => Type.FullName + " constructed";

        private readonly PreinitializationManager _preinitializationManager;
        private readonly NodeFactory _nodeFactory;
        private readonly ModuleDesc _module;

        public override bool ShouldSkipEmitting(NodeFactory factory) => false;

        public ConstructedEETypeNode(TypeDesc type, int baseSize, INameMangler nameMangler, PreinitializationManager preinitializationManager, NodeFactory nodeFactory, ModuleDesc module) 
            : base(type, nameMangler)
        {
            BaseSize = baseSize;
            _preinitializationManager = preinitializationManager;
            _nodeFactory = nodeFactory;
            _module = module;
        }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context)
        {
            var dependencies = new List<IDependencyNode>();
            if (Type.IsSzArray)
            {
                var arrayType = (ArrayType)Type;

                var elemType = arrayType.ElementType;
                if (!elemType.IsInterface)
                {
                    RelatedType = elemType;
                    var elemConstructedEETypeNode = context.NodeFactory.NecessaryTypeSymbol(elemType);
                    dependencies.Add(elemConstructedEETypeNode);
                }

                TypeDesc systemArrayType = (TypeDesc)_module.GetType("System", "Array");
                var allocSize = ((DefType)systemArrayType).InstanceByteCount;
                var constructedEETypeNode = context.NodeFactory.ConstructedEETypeNode(systemArrayType, allocSize.AsInt);
                dependencies.Add(constructedEETypeNode);
            }
            else
            {
                if (Type.HasBaseType)
                {
                    DefType baseType = Type.BaseType!;
                    RelatedType = baseType;

                    if (!baseType.IsValueType)
                    {
                        var allocSize = baseType.InstanceByteCount.AsInt;
                        var constructedEETypeNode = context.NodeFactory.ConstructedEETypeNode(baseType, allocSize);
                        dependencies.Add(constructedEETypeNode);
                    }
                }
            }

            // Enumerate each interface this type implements and add as dependencies
            if (!Type.IsSzArray)
            {
                foreach (var interfaceType in Type.RuntimeInterfaces)
                {
                    var interfaceTypeNode = _nodeFactory.NecessaryTypeSymbol(interfaceType);
                    dependencies.Add(interfaceTypeNode);
                }
            }

            return dependencies;
        }

        public override IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context)
        {
            var defType = Type.Context.GetClosestDefType(Type);

            IList<ConditionalDependency> conditionalDependencies = new List<ConditionalDependency>();
            foreach (var method in VirtualMethodAlgorithm.EnumAllVirtualSlots(defType))
            {
                if (!method.HasGenericParameters)
                {
                    var implementation = VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(defType, method);

                    // Add a conditional dependency if the method implementation is on this type
                    // and is not abstract since there is no code for an abstract method

                    if (implementation?.OwningType == defType && !implementation.IsAbstract)
                    {
                        var conditionalDependency = new ConditionalDependency
                        {
                            IfNode = context.NodeFactory.VirtualMethodUse(method),
                            ThenParent = this,
                            ThenNode = context.NodeFactory.MethodNode(implementation, defType.IsValueType),
                        };
                        conditionalDependencies.Add(conditionalDependency);
                    }
                }
                else
                {
                    throw new NotImplementedException("Generic virtual methods not supported");
                }
            }

            var runtimeInterfaces = defType.RuntimeInterfaces;
            for (int interfaceIndex = 0;  interfaceIndex < runtimeInterfaces.Length; interfaceIndex++) 
            {
                var interfaceType = runtimeInterfaces[interfaceIndex];

                foreach (var method in interfaceType.GetAllVirtualMethods())
                {
                    var implMethod = VirtualMethodAlgorithm.ResolveInterfaceMethodToVirtualMethodOnType(method, defType);
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

            return conditionalDependencies;
        }
        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            var eeMangledTypeName = _nameMangler.GetMangledTypeName(Type);

            var instructionsBuilder = new InstructionsBuilder();
            instructionsBuilder.Comment($"{Type}");

            // Need to mangle full field name here
            instructionsBuilder.Label(eeMangledTypeName);

            // Emit data for EEType flags here
            ushort flags = 0;
            if (_preinitializationManager.HasLazyStaticConstructor(Type))
            {
                flags = 1;
            }
            instructionsBuilder.Dw(flags, "EEType flags");

            // Emit data for EEType here
            var baseSize = BaseSize;

            instructionsBuilder.Dw((ushort)baseSize, "Base Size");

            if (RelatedType != null)
            {
                var relatedTypeMangledTypeName = _nameMangler.GetMangledTypeName(RelatedType);
                instructionsBuilder.Dw(relatedTypeMangledTypeName, "Related Type");
            }
            else
            {
                instructionsBuilder.Dw((ushort)0, "No Related Type");
            }

            var vtableSlotCountReservation = instructionsBuilder.ReserveByte();
            var interfaceCountReservation = instructionsBuilder.ReserveByte();

            var defType = Type.Context.GetClosestDefType(Type);

            // Emit VTable
            OutputVirtualSlots(instructionsBuilder, defType, defType);
            instructionsBuilder.UpdateReservation(vtableSlotCountReservation, Instruction.Create(Opcode.Db, _virtualSlotCount, "VTable slot count"));

            // Emit Interface map
            OutputInterfaceMap(instructionsBuilder, defType);
            instructionsBuilder.UpdateReservation(interfaceCountReservation, Instruction.Create(Opcode.Db, _interfaceSlotCount, "Interface slot count"));

            // Emit dispatch map
            OutputDispatchMap(instructionsBuilder, defType);

            return instructionsBuilder.Instructions;
        }

        private byte _interfaceSlotCount = 0;

        private void OutputInterfaceMap(InstructionsBuilder instructionsBuilder, DefType defType)
        {
            instructionsBuilder.Comment($"Interface map for {Type.FullName}");

            var interfaces = defType.RuntimeInterfaces;

            // Enumerate each interface this type implements
            foreach (var interfaceType in interfaces) 
            {
                var interfaceTypeNode = _nodeFactory.NecessaryTypeSymbol(interfaceType);
                instructionsBuilder.Dw(interfaceTypeNode.MangledTypeName);

                _interfaceSlotCount++;
            }
        }

        private void OutputDispatchMap(InstructionsBuilder instructionsBuilder, DefType defType)
        {
            instructionsBuilder.Comment($"Dispatch map for {defType.FullName}");

            var dispatchMapEntryCountReservation = instructionsBuilder.ReserveByte();

            var interfaces = defType.RuntimeInterfaces;

            byte entryCount = 0;

            // Enumerate each interface this type implements
            for (int interfaceIndex = 0; interfaceIndex < interfaces.Length; interfaceIndex++) 
            {
                var interfaceType = interfaces[interfaceIndex];
                var virtualSlots = _nodeFactory.VTable(interfaceType).GetSlots();

                // For each interface method slot try to emit a dispatch map
                for (int interfaceMethodSlot = 0; interfaceMethodSlot < virtualSlots.Count; interfaceMethodSlot++) 
                {
                    var method = virtualSlots[interfaceMethodSlot];

                    if (method.IsStatic)
                    {
                        // TODO: Static interface methods
                        throw new NotImplementedException("Static interface methods not currently supported");
                    }

                    var implMethod = VirtualMethodAlgorithm.ResolveInterfaceMethodToVirtualMethodOnType(method, defType);
                    if (implMethod != null)
                    {
                        int emittedInterfaceSlot = interfaceMethodSlot;
                        int emittedImplSlot = VirtualMethodSlotHelper.GetVirtualMethodSlot(_nodeFactory, implMethod);

                        instructionsBuilder.Comment($"Interface {interfaceType.FullName}, {method.FullName}, {implMethod.FullName}");
                        instructionsBuilder.Db((byte)interfaceIndex, "Interface index");
                        instructionsBuilder.Db((byte)emittedInterfaceSlot, "Interface slot");
                        instructionsBuilder.Db((byte)emittedImplSlot, "Implementation slot");

                        entryCount++;
                    }
                    else
                    {
                        // No implementation in this type, so must be implemented by a base type in the hierarchy.
                        // No need to emit a dispatch map in this case as the runtime interface dispatch code will
                        // walk the inheritance chain

                        var result = VirtualMethodAlgorithm.ResolveInterfaceMethodToDefaultImplementationOnType(method, interfaceType, out implMethod);

                        if (result != DefaultInterfaceMethodResolution.None)
                        {
                            // TODO: Will need to check here for
                            //  * default implementation
                            //  * Reabstraction
                            //  * Diamond

                            throw new NotImplementedException();
                        }
                    }
                }
            }

            instructionsBuilder.UpdateReservation(dispatchMapEntryCountReservation, Instruction.Create(Opcode.Db, entryCount));
        }

        private byte _virtualSlotCount = 0;

        private void OutputVirtualSlots(InstructionsBuilder instructionsBuilder, DefType type, DefType implType)
        {
            instructionsBuilder.Comment($"VTable slots for {type.FullName}");

            // Output inherited VTable slots first
            var baseType = type.BaseType;
            if (baseType != null)
            {
                OutputVirtualSlots(instructionsBuilder, baseType, implType);
            }

            // Now get new slots
            var vTable = _nodeFactory.VTable(type);
            var virtualSlots = vTable.GetSlots();

            // Emit VTable entries for the new slots
            for (int i = 0; i < virtualSlots.Count; i++)
            {
                var method = virtualSlots[i];
                var implementation = VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(implType, method);

                // Only generate slot entries for non abstract methods
                if (implementation != null && !implementation.IsAbstract)
                {
                    var node = _nodeFactory.MethodNode(implementation, implementation.OwningType.IsValueType);
                    instructionsBuilder.Dw(node.GetMangledName(_nameMangler));
                    _virtualSlotCount++;
                }
            }
        }
    }
}