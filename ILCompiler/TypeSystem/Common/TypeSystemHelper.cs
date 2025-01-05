using ILCompiler.Compiler;

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
            return type == type.Context.GetWellKnownType(wellKnownType, false);
        }

        public static ArrayType MakeArrayType(this TypeDesc type) => type.Context.GetArrayType(type);
    }
}
