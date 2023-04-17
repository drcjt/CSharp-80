using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Common.TypeSystem.IL
{
    public static class UnsafeIntrinsics
    {
        public static CilBody? EmitIL(MethodDesc method)
        {
            switch (method.Name)
            {
                case "As":
                    return EmitAs(method);
                case "AsPointer":
                    return EmitAsPointer(method);
                case "SizeOf":
                    return EmitSizeOf(method);
                case "Add":
                    return EmitAdd(method);
                case "AddByteOffset":
                    return EmitAddByteOffset(method);
                case "InitBlock":
                    return EmitInitBlock(method);
            }

            return null;
        }

        private static CilBody? EmitAs(MethodDesc method) 
        {
            var body = new CilBody();

            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            body.UpdateInstructionOffsets();

            return body;
        }

        private static CilBody? EmitAsPointer(MethodDesc method)
        {
            var body = new CilBody();

            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            body.Instructions.Add(OpCodes.Conv_U.ToInstruction());
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            body.UpdateInstructionOffsets();

            return body;
        }

        private static CilBody? EmitSizeOf(MethodDesc method) 
        {
            var body = new CilBody();

            body.Instructions.Add(OpCodes.Sizeof.ToInstruction(method.ResolveType(new GenericMVar(0)).ToTypeDefOrRef()));
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            body.UpdateInstructionOffsets();

            return body;            
        }

        private static CilBody? EmitInitBlock(MethodDesc method)
        {
            var body = new CilBody();

            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
            body.Instructions.Add(OpCodes.Ldarg_2.ToInstruction());
            body.Instructions.Add(OpCodes.Initblk.ToInstruction());
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            body.UpdateInstructionOffsets();

            return body;
        }

        private static CilBody? EmitAdd(MethodDesc method)
        {
            var body = new CilBody();

            body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
            body.Instructions.Add(OpCodes.Sizeof.ToInstruction(method.ResolveType(new GenericMVar(0)).ToTypeDefOrRef()));
            body.Instructions.Add(OpCodes.Conv_I.ToInstruction());
            body.Instructions.Add(OpCodes.Mul.ToInstruction());
            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            body.Instructions.Add(OpCodes.Add.ToInstruction());
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            body.UpdateInstructionOffsets();

            return body;
        }

        private static CilBody? EmitAddByteOffset(MethodDesc method) 
        {
            var body = new CilBody();

            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
            body.Instructions.Add(OpCodes.Add.ToInstruction(method.ResolveType(new GenericMVar(0)).ToTypeDefOrRef()));
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            body.UpdateInstructionOffsets();

            return body;
        }
    }
}
