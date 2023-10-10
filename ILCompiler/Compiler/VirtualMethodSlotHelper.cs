using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;

namespace ILCompiler.Compiler
{
    public static class VirtualMethodSlotHelper
    {
        public static int GetVirtualMethodSlot(NodeFactory factory, MethodDef method, TypeDef implType)
        {
            var owningType = method.DeclaringType;
            int baseSlots = GetNumberOfBaseSlots(factory, owningType);

            IReadOnlyList<MethodDef> virtualSlots = factory.VTable(owningType).GetSlots();
            int methodSlot = -1;
            for (int slot = 0; slot < virtualSlots.Count; slot++) 
            { 
                if (virtualSlots[slot] == method)
                {
                    methodSlot = slot;
                    break;
                }
            }

            return methodSlot == -1 ? -1 : baseSlots + methodSlot;
        }

        private static int GetNumberOfBaseSlots(NodeFactory factory, TypeDef owningType) 
        {
            int baseSlots = 0;

            var baseType = owningType.BaseType;
            while (baseType != null)
            {
                var resolvedBaseType = baseType.ResolveTypeDefThrow();
                IReadOnlyList<MethodDef> baseVirtualSlots = factory.VTable(resolvedBaseType).GetSlots();
                baseSlots += baseVirtualSlots.Count;

                baseType = baseType.GetBaseType();
            }

            return baseSlots;
        }
    }
}