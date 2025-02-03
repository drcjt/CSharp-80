using ILCompiler.IL;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.Utilities
{
    public static class ArrayOfTRuntimeInterfacesAlgorithm
    {
        public static DefType[] ComputeRuntimeInterfaces(TypeDesc typeDesc)
        {
            var systemModule = typeDesc.Context.SystemModule;

            var arrayOfTType = systemModule!.GetKnownType("System", "Array`1");

            ArrayType arrayType = (ArrayType)typeDesc;
            TypeDesc arrayOfTInstantiation = arrayOfTType.MakeInstantiatedType(arrayType.ElementType);

            return arrayOfTInstantiation.RuntimeInterfaces;
        }
    }
}
