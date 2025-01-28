using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Common.TypeSystem.IL
{
    public static class UnsafeIntrinsics
    {
        public static MethodIL? EmitIL(MethodDesc method)
        {
            switch (method.Name)
            {
                case "As":
                    return EmitAs();
                case "AsPointer":
                    return EmitAsPointer();
                case "SizeOf":
                    return EmitSizeOf(method);
                case "Add":
                    return EmitAdd(method);
                case "AddByteOffset":
                    return EmitAddByteOffset(method);
                case "InitBlock":
                    return EmitInitBlock();
                case "CopyBlock":
                    return EmitCopyBlock();
            }

            return null;
        }

        private static MethodIL? EmitAs() 
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 0));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 1));

            return body;
        }

        private static MethodIL? EmitAsPointer()
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 0));
            body.Instructions.Add(new Instruction(ILOpcode.conv_u, 1));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 2));

            return body;
        }

        private static MethodIL? EmitSizeOf(MethodDesc method) 
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.sizeof_, 0, new SignatureMethodVariable(method.Context, 0)));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 1));

            return body;
        }

        private static MethodIL? EmitInitBlock()
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 0));
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_1, 1));
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_2, 2));
            body.Instructions.Add(new Instruction(ILOpcode.initblk, 3));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 4));

            return body;
        }

        private static MethodIL? EmitCopyBlock()
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 0));
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_1, 1));
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_2, 2));
            body.Instructions.Add(new Instruction(ILOpcode.cpblk, 3));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 4));

            return body;
        }

        private static MethodIL? EmitAdd(MethodDesc method)
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.ldarg_1, 0));
            body.Instructions.Add(new Instruction(ILOpcode.sizeof_, 1, new SignatureMethodVariable(method.Context, 0)));
            body.Instructions.Add(new Instruction(ILOpcode.conv_i, 2));
            body.Instructions.Add(new Instruction(ILOpcode.mul, 3));
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 4));
            body.Instructions.Add(new Instruction(ILOpcode.add, 5));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 6));

            return body;
        }

        private static MethodIL? EmitAddByteOffset(MethodDesc method) 
        {
            var body = new MethodIL();

            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, 0));
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_1, 1));
            body.Instructions.Add(new Instruction(ILOpcode.add, 2));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 3));

            return body;
        }
    }
}
