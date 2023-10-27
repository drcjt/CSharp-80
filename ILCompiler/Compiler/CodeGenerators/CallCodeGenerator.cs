using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CallCodeGenerator : ICodeGenerator<CallEntry>
    {
        public void GenerateCode(CallEntry entry, CodeGeneratorContext context)
        {
            if (entry.Method != null && entry.Method.DeclaringType.IsInterface)
            {
                GenerateCodeForInterfaceCall(entry, context);
            }
            else  if (entry.IsVirtual)
            {
                GenerateCodeForVirtualCall(entry, context);
            }
            else
            {
                GenerateCodeForCall(entry, context);
            }
        }

        private static void GenerateCodeForInterfaceCall(CallEntry entry, CodeGeneratorContext context) 
        {
            if (entry.Method == null)
            {
                throw new InvalidOperationException("Method must not be null for interface dispatch");
            }

            var interfaceEETypeNode = context.NodeFactory.NecessaryTypeSymbol(entry.Method.DeclaringType);
            int methodSlot = VirtualMethodSlotHelper.GetVirtualMethodSlot(context.NodeFactory, entry.Method);

            // Stack holds this pointer and actual parameters for method call
            // Pass other parameters in registers:

            context.InstructionsBuilder.Ld(BC, interfaceEETypeNode.MangledTypeName);
            context.InstructionsBuilder.Ld(DE, (ushort)methodSlot);

            // IntefaceCall searches dispatch map looking for entry with required method slot
            // If found it checks if the interfaceindex matches too - based on finding the interfaceType in the interface map
            // If found then the implementation slot is used with the vtable to find the implementation method and jump to it
            // Otherwise continue searching the dispatch map
            // If no match in dispatch map then recurse up inheritance hierarchy via base type and repeat above

            context.InstructionsBuilder.Call("InterfaceCall");
        }

        private static void GenerateCodeForCall(CallEntry entry, CodeGeneratorContext context)
        {
            if (entry.Method != null && entry.Method.IsInternalCall && entry.Arguments.Count > 0)
            {
                // Pass last argument in HL for Ptr type
                // and in HL, DE otherwise
                var argument = entry.Arguments[entry.Arguments.Count - 1];
                context.InstructionsBuilder.Pop(HL);

                if (argument.Type != VarType.Ptr && argument.Type != VarType.Ref) // TODO: should this also check for ByRef??
                {
                    context.InstructionsBuilder.Pop(DE);
                }
            }
            context.InstructionsBuilder.Call(entry.TargetMethod);
        }

        private static void GenerateCodeForVirtualCall(CallEntry entry, CodeGeneratorContext context)
        {
            var targetMethod = entry.Method;

            if (targetMethod == null)
            {
                throw new InvalidOperationException("Virtual CallEntry must have non-null Method");
            }

            int slot = VirtualMethodSlotHelper.GetVirtualMethodSlot(context.NodeFactory, targetMethod);

            // EEType header comprises of following:
            //    2 bytes for Flags
            //    2 bytes for base size
            //    2 bytes for related type
            //    1 byte for vtable slot count
            //    1 byte for interface slot count
            const int eeTypeHeader = 2 + 2 + 2 + 1 + 1;
            context.InstructionsBuilder.Ld(BC, (byte)((slot * 2) + eeTypeHeader));

            context.InstructionsBuilder.Call("VirtualCall");
        }
    }
}
