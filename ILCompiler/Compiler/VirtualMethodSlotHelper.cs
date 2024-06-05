using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;

namespace ILCompiler.Compiler
{
    public static class VirtualMethodSlotHelper
    {
        public static int GetVirtualMethodSlot(NodeFactory factory, MethodDesc method)
        {
            int baseSlots = GetNumberOfBaseSlots(factory, method.OwningType);

            IReadOnlyList<MethodDesc> virtualSlots = factory.VTable(method.OwningType).GetSlots();
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

        private static int GetNumberOfBaseSlots(NodeFactory factory, TypeDesc owningType) 
        {
            int baseSlots = 0;

            var baseType = owningType.BaseType;
            while (baseType != null)
            {
                IReadOnlyList<MethodDesc> baseVirtualSlots = factory.VTable(baseType).GetSlots();
                baseSlots += baseVirtualSlots.Count;

                baseType = baseType.BaseType;
            }

            return baseSlots;
        }
    }
}