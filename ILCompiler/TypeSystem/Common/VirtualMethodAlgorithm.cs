namespace ILCompiler.TypeSystem.Common
{
    /// <summary>
    /// This static methods in this class implement the standard virtual method algorithm as described in ECMA 335.
    /// Specifically see these sections
    ///     II.10.3 Introducing and overriding virtual methods
    ///     II.10.3.1 Introduction a virtual method
    /// </summary>
    public static class VirtualMethodAlgorithm
    {
        /// <summary>
        /// Enumerate all virtual methods on the specified type and it's parent types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MethodDesc> EnumAllVirtualSlots(TypeDesc type)
        {
            var alreadyEnumerated = new List<MethodDesc>();
            if (!type.IsInterface)
            {
                TypeDesc? currentType = type;
                do
                {
                    foreach (var method in currentType.GetVirtualMethods())
                    {
                        if (method.IsVirtual)
                        {
                            var possibleVirtual = FindSlotDefiningMethodForVirtualMethod(method);
                            if (!alreadyEnumerated.Contains(possibleVirtual))
                            {
                                alreadyEnumerated.Add(possibleVirtual);
                                yield return possibleVirtual;
                            }
                        }
                    }
                    currentType = currentType.HasBaseType ? currentType.BaseType : null;
                } while (currentType != null);
            }
        }

        /// <summary>
        /// Resolve a virtual method call on the specified type
        /// </summary>
        /// <param name="decl"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodDesc? FindVirtualFunctionTargetMethodOnObjectType(TypeDesc? currentType, MethodDesc method)
        {
            while (currentType != null)
            {
                var nameSigOverride = FindMatchingVirtualMethodOnTypeByNameAndSig(method, currentType);
                if (nameSigOverride != null)
                {
                    return nameSigOverride;
                }

                // No match so move up the type hierarchy and try again
                currentType = currentType.HasBaseType ? currentType.BaseType : null;
            }

            return null;
        }

        /// <summary>
        /// Find matching virtual method by name and signature on a type. Finds both exact and equivalent methods.
        /// Prefers exact match over equivalent match.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        public static MethodDesc? FindMatchingVirtualMethodOnTypeByNameAndSig(MethodDesc targetMethod, TypeDesc currentType)
        {
            MethodDesc? exactMatch = null;
            MethodDesc? equivalentMatch = null;

            foreach (var candidateMethod in currentType.GetAllVirtualMethods())
            {
                if (candidateMethod.Name == targetMethod.Name && candidateMethod.Signature.EquivalentTo(targetMethod.Signature))
                {
                    equivalentMatch = candidateMethod;

                    if (candidateMethod.Signature == targetMethod.Signature)
                    {
                        exactMatch = candidateMethod;
                    }
                }
            }

            return exactMatch ?? equivalentMatch;
        }

        /// <summary>
        /// Find the base type method that defines the slot for the specified method.
        /// This is either:
        ///    * the new slot method most derived that is in the parent hierarchy of the method, or
        ///    * the least derived method that isn't new slot that matches by name and sig
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodDesc FindSlotDefiningMethodForVirtualMethod(MethodDesc method)
        {
            var owningType = method.OwningType;
            var currentType = owningType.HasBaseType ? owningType.BaseType : null;

            while (currentType != null && !method.IsNewSlot)
            {
                var foundMethod = FindMatchingVirtualMethodOnTypeByNameAndSig(method, currentType);
                if (foundMethod != null)
                {
                    return foundMethod;
                }

                currentType = currentType.HasBaseType ? currentType.BaseType : null;
            }

            return method;
        }

        public static MethodDesc? ResolveInterfaceMethodToVirtualMethodOnType(MethodDesc interfaceMethod, TypeDesc currentType)
        {
            return ResolveInterfaceMethodToVirtualMethodOnType(interfaceMethod, (MetadataType)currentType);
        }


        /// <summary>
        /// Interface resolution.
        /// See ECMA II.12.2
        /// </summary>
        /// <param name="interfaceMethod"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        public static MethodDesc? ResolveInterfaceMethodToVirtualMethodOnType(MethodDesc interfaceMethod, MetadataType currentType) 
        {
            if (!currentType.IsInterface)
            {
                var methodImpl = FindImplFromDeclFromMethodImpls(currentType, interfaceMethod);
                if (methodImpl != null)
                {
                    return methodImpl;
                }

                var interfaceType = (DefType)interfaceMethod.OwningType;
                var baseType = currentType.MetadataBaseType;

                var foundExplicitInterface = IsInterfaceExplicitlyImplementedOnType(currentType, interfaceType);
                if (foundExplicitInterface)
                {
                    var foundOnCurrentType = FindMatchingVirtualMethodOnTypeByNameAndSig(interfaceMethod, currentType);
                    if (foundOnCurrentType != null)
                    {
                        foundOnCurrentType = FindSlotDefiningMethodForVirtualMethod(foundOnCurrentType);
                    }

                    if (baseType == null)
                        return foundOnCurrentType;

                    if (foundOnCurrentType == null && ResolveInterfaceMethodToVirtualMethodOnType(interfaceMethod, baseType) == null)
                    {
                        if (!IsInterfaceImplementedOnType(baseType, interfaceType))
                        {
                            throw new NotImplementedException("Unable to resolve explicit interface method to virtual method on type");
                        }
                    }

                    return foundOnCurrentType;
                }
                else if (IsInterfaceImplementedOnType(currentType, interfaceType) && 
                         ResolveInterfaceMethodToVirtualMethodOnTypeRecursive(interfaceMethod, baseType) == null)
                {
                    var foundOnCurrentType = FindMatchingVirtualMethodOnTypeByNameAndSig(interfaceMethod, currentType);
                    if (foundOnCurrentType != null)
                    {
                        foundOnCurrentType = FindSlotDefiningMethodForVirtualMethod(foundOnCurrentType);
                    }

                    if (foundOnCurrentType != null)
                    {
                        return foundOnCurrentType;
                    }

                    throw new NotImplementedException("Unable to resolve implicit interface method to virtual method on type");
                }
            }

            return null;
        }

        private static MethodDesc? FindImplFromDeclFromMethodImpls(MetadataType type, MethodDesc decl)
        {
            MethodImplRecord[] foundMethodImpls = type.FindMethodsImplWithMatchingDeclName(decl.Name);

            if (foundMethodImpls == null)
                return null;

            bool interfaceDecl = decl.OwningType.IsInterface;

            foreach (MethodImplRecord record in foundMethodImpls)
            {
                MethodDesc recordDecl = record.Decl;

                if (interfaceDecl != recordDecl.OwningType.IsInterface)
                    continue;

                if (!interfaceDecl)
                    recordDecl = FindSlotDefiningMethodForVirtualMethod(recordDecl);

                if (recordDecl == decl)
                {
                    return FindSlotDefiningMethodForVirtualMethod(record.Body);
                }
            }

            return null;
        }

        public static DefaultInterfaceMethodResolution ResolveInterfaceMethodToDefaultImplementationOnType(MethodDesc interfaceMethod, TypeDesc currentType, out MethodDesc? impl)
        {
            var interfaceMethodOwningType = interfaceMethod.OwningType;
            DefType[] consideredInterfaces;
            if (currentType.IsInterface)
            {
                consideredInterfaces = currentType.RuntimeInterfaces;
            }
            else
            {
                consideredInterfaces = new DefType[currentType.RuntimeInterfaces.Length + 1];
                Array.Copy(currentType.RuntimeInterfaces, consideredInterfaces, currentType.RuntimeInterfaces.Length);
                consideredInterfaces[consideredInterfaces.Length - 1] = (DefType)currentType;
            }

            impl = null;

            DefType? mostSpecificInterface = null;
            bool diamondCase = false;
            foreach (MetadataType runtimeInterface in consideredInterfaces)
            {
                if (runtimeInterface == interfaceMethodOwningType)
                {
                    if (mostSpecificInterface == null && !interfaceMethod.IsAbstract)
                    {
                        mostSpecificInterface = runtimeInterface;
                        impl = interfaceMethod;
                    }
                }
                else if (Array.IndexOf(runtimeInterface.RuntimeInterfaces, interfaceMethodOwningType) != 1)
                {
                    var possibleMethodOverrides = runtimeInterface.FindMethodsImplWithMatchingDeclName(interfaceMethod.FullName);
                    if (possibleMethodOverrides != null)
                    {
                        foreach (var methodOverride in possibleMethodOverrides) 
                        {
                            if (methodOverride.Decl == interfaceMethod)
                            {
                                if (mostSpecificInterface == null || Array.IndexOf(runtimeInterface.RuntimeInterfaces, mostSpecificInterface) != 1)
                                {
                                    mostSpecificInterface = runtimeInterface;
                                    impl = methodOverride.Body;
                                    diamondCase = false;
                                }
                                else if (Array.IndexOf(mostSpecificInterface.RuntimeInterfaces, runtimeInterface) == -1)
                                {
                                    diamondCase = true;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            if (diamondCase)
            {
                impl = null;
                return DefaultInterfaceMethodResolution.Diamond;
            }
            else if (impl == null)
            {
                return DefaultInterfaceMethodResolution.None;
            }
            else if (impl.IsAbstract)
            {
                impl = null;
                return DefaultInterfaceMethodResolution.Reabstraction;
            }

            return DefaultInterfaceMethodResolution.DefaultImplementation;
        }

        private static MethodDesc? ResolveInterfaceMethodToVirtualMethodOnTypeRecursive(MethodDesc interfaceMethod, MetadataType? currentType)
        {
            while (currentType != null && IsInterfaceImplementedOnType(currentType, (DefType)(interfaceMethod.OwningType)))
            {
                var typeInterfaceResolution = ResolveInterfaceMethodToVirtualMethodOnType(interfaceMethod, currentType);
                if (typeInterfaceResolution != null)
                {
                    return typeInterfaceResolution;
                }
                currentType = currentType.MetadataBaseType;
            }
            return null;
        }

        /// <summary>
        /// Determine if a type implements an interface either explicitly or via inheritance
        /// </summary>
        /// <param name="currentType"></param>
        /// <param name="interfaceType"></param>
        /// <returns>true if interface is implemented by the type</returns>
        private static bool IsInterfaceImplementedOnType(DefType currentType, DefType interfaceType)
        {
            var runtimeInterfaces = MetadataRuntimeInterfacesAlgorithm.ComputeRuntimeInterfaces(currentType);
            return runtimeInterfaces.Any(type => type == interfaceType);
        }

        /// <summary>
        /// Determine if a type implements an interface explicitly
        /// </summary>
        /// <param name="currentType"></param>
        /// <param name="interfaceType"></param>
        /// <returns>true if interface is implemented by the type explicitly</returns>
        private static bool IsInterfaceExplicitlyImplementedOnType(MetadataType currentType, DefType interfaceType)
            => currentType.ExplicitlyImplementedInterfaces.Any(interfaceImpl => interfaceImpl == interfaceType);
    }

    public struct MethodImplRecord
    {
        public readonly MethodDesc Decl;
        public readonly MethodDesc Body;

        public MethodImplRecord(MethodDesc decl, MethodDesc body)
        {
            Decl = decl;
            Body = body;
        }
    }

}
