namespace ILCompiler.TypeSystem.Common
{
    public static class TypeSystemHelper
    {
        public static LayoutInt GetElementSize(this TypeDesc type)
        {
            if (type.IsValueType)
            {
                return ((DefType)type).InstanceFieldSize;
            }
            else
            {
                return type.Context.Target.LayoutPointerSize;
            }
        }

        public static IEnumerable<MethodDesc> GetAllVirtualMethods(this TypeDesc type)
        {
            return type.Context.GetAllVirtualMethods(type);
        }

        public static bool IsWellKnownType(this TypeDesc type, WellKnownType wellKnownType)
        {
            return type == type.Context.GetWellKnownType(wellKnownType);
        }

        public static ArrayType MakeArrayType(this TypeDesc type) => type.Context.GetArrayType(type);

        public static MethodDesc? FindMethodOnTypeWithMatchingTypicalMethod(this TypeDesc targetType, MethodDesc method)
        {
            if (!method.HasInstantiation && !method.OwningType.HasInstantiation)
            {
                return method;
            }

            var typicalTypeOfTargetMethod = method.GetTypicalMethodDefinition().OwningType;
            TypeDesc? targetOrBase = targetType;
            do
            {
                var openTargetOrBase = targetOrBase;
                if (openTargetOrBase is InstantiatedType)
                {
                    openTargetOrBase = openTargetOrBase.GetTypeDefinition();
                }
                if (openTargetOrBase == typicalTypeOfTargetMethod)
                {
                    return targetOrBase.FindMethodOnExactTypeWithMatchingTypicalMethod(method);
                }
                targetOrBase = targetOrBase.BaseType;
            } while (targetOrBase != null);

            return null;
        }

        internal static MethodDesc FindMethodOnExactTypeWithMatchingTypicalMethod(this TypeDesc type, MethodDesc method)
        {
            var methodTypicalDefinition = method.GetTypicalMethodDefinition();

            var instantiatedType = type as InstantiatedType;
            if (instantiatedType != null)
            {
                return method.Context.GetMethodForInstantiatedType(methodTypicalDefinition, instantiatedType);
            }
            else if (type.IsArray)
            {
                throw new NotImplementedException();
            }
            else
            {
                return methodTypicalDefinition;
            }
        }

        public static MethodDesc? FindVirtualFunctionTargetMethodOnObjectType(this TypeDesc? type, MethodDesc targetMethod)
        {
            return VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(type, targetMethod);
        }

        public static MethodDesc? ResolveInterfaceMethodToVirtualMethodOnType(this TypeDesc type, MethodDesc interfaceMethod)
        {
            return VirtualMethodAlgorithm.ResolveInterfaceMethodToVirtualMethodOnType(interfaceMethod, type);
        }

        public static InstantiatedMethod MakeInstantiatedMethod(this MethodDesc methodDef, Instantiation instantiation)
        {
            return methodDef.Context.GetInstantiatedMethod(methodDef, instantiation);
        }

        public static InstantiatedType MakeInstantiatedType(this MetadataType typeDef, params TypeDesc[] genericParameters)
        {
            return typeDef.Context.GetInstantiatedType(typeDef, new Instantiation(genericParameters));
        }
    }
}
