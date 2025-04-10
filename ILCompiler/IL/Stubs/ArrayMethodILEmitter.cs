using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using ILCompiler.TypeSystem.IL.Stubs;
using System.Diagnostics;

namespace ILCompiler.IL.Stubs
{
    public class ArrayMethodILEmitter
    {
        private readonly ArrayMethod _method;
        private readonly ILEmitter _emitter;
        
        private ArrayMethodILEmitter(ArrayMethod method)
        {
            _method = method;
            _emitter = new ILEmitter();
        }

        public static MethodIL EmitIL(ArrayMethod arrayMethod)
        {
            return new ArrayMethodILEmitter(arrayMethod).EmitIL();
        }

        private MethodIL EmitIL()
        {
            switch (_method.Kind)
            {
                case ArrayMethodKind.Get:
                case ArrayMethodKind.Set:
                case ArrayMethodKind.AddressWithHiddenArg:
                    EmitILForAccessor();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return _emitter.Link();
        }

        private void EmitILForAccessor()
        {
            Debug.Assert(_method.OwningType.IsMdArray);

            var context = _method.Context;
            int pointerSize = context.Target.PointerSize;

            int argStartOffset = _method.Kind == ArrayMethodKind.AddressWithHiddenArg ? 2 : 1;

            ArrayType arrayType = (ArrayType)_method.OwningType;
            var rank = arrayType.Rank;
            var elementType = arrayType.ElementType;

            var helperField = _method.Context.GetWellKnownType(WellKnownType.Object).GetKnownField("m_pEEType");

            var codeStream = _emitter.NewCodeStream();

            var totalLocal = _emitter.NewLocal(context.GetWellKnownType(WellKnownType.IntPtr), "total");
            var lengthLocal = _emitter.NewLocal(context.GetWellKnownType(WellKnownType.IntPtr), "length");

            // TODO: ArrayTypeMismatchException checks

            if (rank == 1)
            {
                throw new NotImplementedException("Methods on rank 1 MdArray not yet implemented");
            }

            var skipRangeCheck = context.Configuration.SkipArrayBoundsCheck;

            var rangeExceptionLabel = ILEmitter.NewCodeLabel();

            for (int i = 0; i < rank; i++)
            {
                // First field is EETypePtr, then we have total length, followed by the lengths of each dimension
                int lengthOffset = (2 * pointerSize + i * 2 /* sizeof(nint) */);

                EmitLoadInteriorAddress(codeStream, helperField, lengthOffset);
                codeStream.Emit(ILOpcode.ldind_i);
                codeStream.EmitStLoc(lengthLocal);

                codeStream.EmitLdArg(new ParameterDefinition(_method.OwningType, "", i + argStartOffset));
                codeStream.Emit(ILOpcode.conv_i);

                if (!skipRangeCheck)
                {
                    codeStream.Emit(ILOpcode.dup);
                    codeStream.EmitLdLoc(lengthLocal);
                    codeStream.Emit(ILOpcode.conv_i);
                    codeStream.Emit(ILOpcode.bge_un, rangeExceptionLabel);
                }

                if (i > 0)
                {
                    codeStream.EmitLdLoc(totalLocal);
                    codeStream.EmitLdLoc(lengthLocal);
                    codeStream.Emit(ILOpcode.mul);
                    codeStream.Emit(ILOpcode.add);
                }

                codeStream.EmitStLoc(totalLocal);
            }

            // Compute offset of first element in the array
            // First field is EETypePtr, then we have total length, followed by the lengths of each dimension
            int firstElementOffset = (2 * pointerSize + 1 * rank * 2 /* sizeof(nint) */);

            EmitLoadInteriorAddress(codeStream, helperField, firstElementOffset);

            codeStream.EmitLdLoc(totalLocal);
            codeStream.Emit(ILOpcode.conv_u);

            int elementSize = elementType.GetElementSize().AsInt;
            if (elementSize != 1)
            {
                codeStream.EmitLdcI4(elementSize);
                codeStream.Emit(ILOpcode.mul);
            }
            codeStream.Emit(ILOpcode.add);

            switch (_method.Kind)
            {
                case ArrayMethodKind.Get:
                    codeStream.Emit(ILOpcode.ldobj, elementType);
                    break;

                case ArrayMethodKind.Set:
                    codeStream.EmitLdArg(new ParameterDefinition(_method.OwningType, "", argStartOffset + rank));
                    codeStream.Emit(ILOpcode.stobj, elementType);
                    break;

                case ArrayMethodKind.AddressWithHiddenArg:
                    break;
            }

            codeStream.Emit(ILOpcode.ret);

            if (!skipRangeCheck)
            {
                codeStream.EmitLabel(rangeExceptionLabel);
                codeStream.Emit(ILOpcode.pop);

                var throwHelperMethod = _method.Context.GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException");
                codeStream.Emit(ILOpcode.call, throwHelperMethod);
            }
        }

        private static void EmitLoadInteriorAddress(ILCodeStream codeStream, FieldDesc helperField, int lengthOffset)
        {
            codeStream.Emit(ILOpcode.ldarg_0); // this pointer
            codeStream.EmitLdFlda(helperField); 
            codeStream.EmitLdcI4(lengthOffset);
            codeStream.Emit(ILOpcode.add);
        }
    }
}
