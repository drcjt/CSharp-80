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
    }
}
