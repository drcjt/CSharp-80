using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler
{
    internal static class TypeExtensions
    {
        public static bool IsCanonicalDefinitionType(this TypeDesc type, CanonicalFormKind kind) => type.Context.IsCanonicalDefinitionType(type, kind);

        public static MethodDesc? TryResolveConstraintMethodApprox(this TypeDesc constrainedType, TypeDesc interfaceType, MethodDesc interfaceMethod)
        {
            bool isStaticVirtualMethod = interfaceMethod.Signature.IsStatic;

            // Don't try resolving calls for reference types
            if (!constrainedType.IsValueType && (!isStaticVirtualMethod || constrainedType.IsCanonicalDefinitionType(CanonicalFormKind.Any)))
            {
                return null;
            }

            Instantiation methodInstantiation = interfaceMethod.Instantiation;
            interfaceMethod = interfaceMethod.GetCanonMethodTarget(CanonicalFormKind.Specific);

            // Find the method that would implement the constraint if we were making the call on a boxed value type
            TypeDesc canonType = constrainedType.ConvertToCanonForm(CanonicalFormKind.Specific);

            MethodDesc genInterfaceMethod = interfaceMethod.GetMethodDefinition();
            MethodDesc? method;
            if (genInterfaceMethod.OwningType.IsInterface)
            {
                // Find all potential interface implementations
                method = EnumeratePotentialInterfaceImplementations(canonType, interfaceType, interfaceMethod, constrainedType, out int potentialMatchingInterfaces);

                if (potentialMatchingInterfaces > 1)
                {
                    // TODO: Consider if constrained type can be cast to the interface type
                    // Three scenarios here, non static virtual methods, static virtual methods with explicit implementation,
                    // static virtual methods with default implementation

                    throw new NotImplementedException();
                }
                else
                {
                    // TODO: Consider if constrained type can be cast to the interface type
                    // Three scenarios here, non static virtual methods, static virtual methods with explicit implementation,
                    // static virtual methods with default implementation
                }
            }
            else if (genInterfaceMethod.IsVirtual)
            {
                MethodDesc? targetMethod = interfaceType.FindMethodOnTypeWithMatchingTypicalMethod(genInterfaceMethod);
                method = constrainedType.FindVirtualFunctionTargetMethodOnObjectType(targetMethod!);
            }
            else
            {
                method = null;
            }

            if (method == null)
            {
                return null;
            }

            // Only return a method if the value type declares the method.
            // This prevents us from returning a method from Object or ValueType
            if (!isStaticVirtualMethod && !method.OwningType.IsValueType)
            {
                return null;
            }

            if (methodInstantiation.Length != 0)
            {
                method = method.MakeInstantiatedMethod(methodInstantiation);
            }

            if (method.IsCanonicalMethod(CanonicalFormKind.Any) && !method.OwningType.IsValueType)
            {
                return null;
            }

            return method;
        }

        private static MethodDesc? EnumeratePotentialInterfaceImplementations(TypeDesc canonType, TypeDesc interfaceType, MethodDesc interfaceMethod, TypeDesc constrainedType, out int potentialMatchingInterfaces)
        {
            MethodDesc genInterfaceMethod = interfaceMethod.GetMethodDefinition();
            bool isStaticVirtualMethod = interfaceMethod.Signature.IsStatic;
            var context = constrainedType.Context;

            MethodDesc? method = null;
            potentialMatchingInterfaces = 0;
            foreach (DefType potentialInterfaceType in canonType.RuntimeInterfaces)
            {
                if (potentialInterfaceType.ConvertToCanonForm(CanonicalFormKind.Specific) == interfaceType.ConvertToCanonForm(CanonicalFormKind.Specific))
                {
                    potentialMatchingInterfaces++;

                    if (isStaticVirtualMethod)
                        continue;

                    // Try and prevent the match from requiring boxing
                    MethodDesc potentialInterfaceMethod = genInterfaceMethod;
                    if (potentialInterfaceMethod.OwningType != potentialInterfaceType)
                    {
                        potentialInterfaceMethod = context.GetMethodForInstantiatedType(potentialInterfaceMethod.GetTypicalMethodDefinition(), (InstantiatedType)potentialInterfaceType);
                    }

                    method = canonType.ResolveInterfaceMethodToVirtualMethodOnType(potentialInterfaceMethod);

                    // Avoid trying to return the parent method
                    if (method != null && !method.OwningType.IsValueType)
                    {
                        return null;
                    }
                }
            }

            return method;
        }
    }
}
