using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common
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
        public static IEnumerable<MethodDef> EnumAllVirtualSlots(TypeDef type)
        {
            var alreadyEnumerated = new List<MethodDef>();
            if (!type.IsInterface)
            {
                TypeDef? currentType = type;
                do
                {
                    foreach (var method in currentType.Methods)
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
                    currentType = currentType.BaseType?.ResolveTypeDefThrow();
                } while (currentType != null);
            }
        }

        /// <summary>
        /// Resolve a virtual method call on the specified type
        /// </summary>
        /// <param name="decl"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodDef? FindVirtualFunctionTargetMethodOnObjectType(TypeDef? currentType, MethodDef method)
        {
            while (currentType != null)
            {
                var nameSigOverride = FindMatchingVirtualMethodOnTypeByNameAndSig(method, currentType);
                if (nameSigOverride != null)
                {
                    return nameSigOverride;
                }

                // No match so move up the type hierarchy and try again
                currentType = currentType.BaseType?.ResolveTypeDefThrow();
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
        public static MethodDef? FindMatchingVirtualMethodOnTypeByNameAndSig(MethodDef targetMethod, TypeDef currentType)
        {
            MethodDef? exactMatch = null;
            MethodDef? equivalentMatch = null;

            foreach (var candidateMethod in currentType.Methods)
            {
                if (candidateMethod.Name == targetMethod.Name && new SigComparer().Equals(candidateMethod.Signature, targetMethod.Signature))
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
        public static MethodDef FindSlotDefiningMethodForVirtualMethod(IMethodDefOrRef methodDefOrRef)
        {
            var method = methodDefOrRef.ResolveMethodDefThrow();
            var currentType = method.DeclaringType.BaseType;

            while (currentType != null && !method.IsNewSlot)
            {
                var foundMethod = FindMatchingVirtualMethodOnTypeByNameAndSig(method, currentType.ResolveTypeDefThrow());
                if (foundMethod != null)
                {
                    return foundMethod;
                }

                currentType = currentType.GetBaseType();
            }

            return method;
        }

        /// <summary>
        /// Interface resolution.
        /// See ECMA II.12.2
        /// </summary>
        /// <param name="interfaceMethod"></param>
        /// <param name="currentType"></param>
        /// <returns></returns>
        public static MethodDef? ResolveInterfaceMethodToVirtualMethodOnType(MethodDef interfaceMethod, TypeDef currentType) 
        {
            if (!currentType.IsInterface)
            {
                var methodImpl = FindMethodsFromDeclarationsFromMethodOverrides(currentType, interfaceMethod);
                if (methodImpl != null)
                {
                    return methodImpl;
                }

                var interfaceType = interfaceMethod.DeclaringType;
                var baseType = currentType.BaseType;

                var foundExplicitInterface = IsInterfaceExplicitlyImplementedOnType(currentType, interfaceType);
                if (foundExplicitInterface)
                {
                    var foundOnCurrentType = FindMatchingVirtualMethodOnTypeByNameAndSig(interfaceMethod, currentType);
                    if (foundOnCurrentType != null)
                    {
                        foundOnCurrentType = FindSlotDefiningMethodForVirtualMethod(foundOnCurrentType);
                    }

                    if (baseType != null && foundOnCurrentType == null)
                    {
                        throw new NotImplementedException("Unable to resolve explicit interface method to virtual method on type");
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

        private static MethodDef? ResolveInterfaceMethodToVirtualMethodOnTypeRecursive(MethodDef interfaceMethod, ITypeDefOrRef? currentType)
        {
            MethodDef? typeInterfaceResolution = null;
            while (currentType != null && typeInterfaceResolution != null && 
                   IsInterfaceImplementedOnType(currentType, interfaceMethod.DeclaringType))
            {
                typeInterfaceResolution = ResolveInterfaceMethodToVirtualMethodOnType(interfaceMethod, currentType.ResolveTypeDefThrow());
                currentType = currentType.GetBaseType();
            }
            return typeInterfaceResolution;
        }

        /// <summary>
        /// Determine if a type implements an interface either explicitly or via inheritance
        /// </summary>
        /// <param name="currentType"></param>
        /// <param name="interfaceType"></param>
        /// <returns>true if interface is implemented by the type</returns>
        private static bool IsInterfaceImplementedOnType(ITypeDefOrRef currentType, TypeDef interfaceType)
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
        private static bool IsInterfaceExplicitlyImplementedOnType(TypeDef currentType, TypeDef interfaceType)
            => currentType.Interfaces.Any(interfaceImpl => interfaceImpl.Interface == interfaceType);


        private static MethodDef? FindMethodsFromDeclarationsFromMethodOverrides(TypeDef currentType, MethodDef method)
        {
            var foundMethodOverrides = FindMethodOverridesWithMatchingDeclarationName(method.Name, currentType);
            if (foundMethodOverrides != null)
            {
                bool isInterfaceDeclaration = method.DeclaringType.IsInterface;

                foreach (var methodOverride in foundMethodOverrides)
                {
                    var declaration = methodOverride.MethodDeclaration.ResolveMethodDefThrow();
                    if (isInterfaceDeclaration == declaration.DeclaringType.IsInterface)
                    {
                        if (!isInterfaceDeclaration)
                        {
                            declaration = FindSlotDefiningMethodForVirtualMethod(declaration);
                        }

                        if (declaration == method)
                        {
                            return FindSlotDefiningMethodForVirtualMethod(methodOverride.MethodBody);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determine the method implementations where the declaration 
        /// FindMethodsWithMatchingOverrideNamess
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<MethodOverride> FindMethodOverridesWithMatchingDeclarationName(string name, TypeDef type)
        {
            var foundRecords = new List<MethodOverride>();
            foreach (var method in type.Methods)
            {
                foreach (var methodOverride in method.Overrides)
                {
                    if (methodOverride.MethodDeclaration.Name == name)
                    {
                        foundRecords.Add(new MethodOverride(method, methodOverride.MethodDeclaration));
                    }
                }
            }

            return foundRecords;
        }
    }
}
