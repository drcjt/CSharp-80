using dnlib.DotNet;
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

            var interfaceType = entry.Method.DeclaringType;
            var resolvedInterfaceType = interfaceType.ResolveTypeDefThrow();

            int methodSlot = VirtualMethodSlotHelper.GetVirtualMethodSlot(context.NodeFactory, entry.Method);

            // Need to generate call to generic resolver runtime routine passing
            // * this pointer for object method is being called on - note this should be on stack already
            // * EEType for interfaceType
            // * methodSlot 
            //
            // resolver will need to use interfacemap via this pointer to convert interfaceType to interface index
            // Then will need to use dispatchmaps via this pointer looking for match with interface index/methodslot
            // If not found then should recurse up inheritance hierarchy via this pointer & related type.
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
