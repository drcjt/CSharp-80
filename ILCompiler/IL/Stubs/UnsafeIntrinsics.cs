using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using ILCompiler.TypeSystem.IL.Stubs;

namespace ILCompiler.Common.TypeSystem.IL
{
    public static class UnsafeIntrinsics
    {
        public static MethodIL? EmitIL(MethodDesc method)
        {
            switch (method.Name)
            {
                case "As":
                case "AsRef":
                    return EmitAs();
                case "AsPointer":
                    return EmitAsPointer();
                case "SizeOf":
                    return EmitSizeOf(method);
                case "Add":
                    return EmitAdd(method);
                case "AddByteOffset":
                    return EmitAddByteOffset();
                case "InitBlock":
                    return EmitInitBlock();
                case "CopyBlock":
                    return EmitCopyBlock();
                case "AreSame":
                    return EmitAreSame();
            }

            return null;
        }

        private static MethodIL? EmitAs() 
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitAsPointer()
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.conv_u);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitSizeOf(MethodDesc method) 
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.sizeof_, new SignatureMethodVariable(method.Context, 0));
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitInitBlock()
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.ldarg_1);
            codeStream.Emit(ILOpcode.ldarg_2);
            codeStream.Emit(ILOpcode.initblk);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitCopyBlock()
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.ldarg_1);
            codeStream.Emit(ILOpcode.ldarg_2);
            codeStream.Emit(ILOpcode.cpblk);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitAdd(MethodDesc method)
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_1);
            codeStream.Emit(ILOpcode.sizeof_, new SignatureMethodVariable(method.Context, 0));
            codeStream.Emit(ILOpcode.conv_i);
            codeStream.Emit(ILOpcode.mul);
            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.add);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitAddByteOffset() 
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.ldarg_1);
            codeStream.Emit(ILOpcode.add);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }

        private static MethodIL? EmitAreSame()
        {
            var emitter = new ILEmitter();
            var codeStream = emitter.NewCodeStream();

            codeStream.Emit(ILOpcode.ldarg_0);
            codeStream.Emit(ILOpcode.ldarg_1);
            codeStream.Emit(ILOpcode.ceq);
            codeStream.Emit(ILOpcode.ret);

            return emitter.Link();
        }
    }
}
