using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common
{
    /// <summary>
    /// This static methods in this class implement the standard virtual method algorithm as described in ECMA 335.
    /// Specifically see these sections
    ///     II.10.3 Introducing and overriding virtual methods
    ///     II.10.3.1 Introduction a virtual method
    /// </summary>
    public class VirtualMethodAlgorithm
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
                if (candidateMethod.Name == targetMethod.Name)
                {
                    if (new SigComparer().Equals(candidateMethod.Signature, targetMethod.Signature))
                    {
                        equivalentMatch = candidateMethod;

                        if (candidateMethod.Signature == targetMethod.Signature)
                        {
                            exactMatch = candidateMethod;
                        }
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
        public static MethodDef FindSlotDefiningMethodForVirtualMethod(MethodDef method)
        {
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
    }
}
