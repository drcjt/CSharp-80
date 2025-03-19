using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.IL.Stubs
{
    public class ArrayMethodILEmitter
    {
        private readonly ArrayMethod _method;
        private readonly LocalVariableDefinition _totalLocal;
        private readonly LocalVariableDefinition _lengthLocal;
            
        private ArrayMethodILEmitter(ArrayMethod method, IList<LocalVariableDefinition> locals)
        {
            _method = method;
            var context = _method.Context;

            _totalLocal = new LocalVariableDefinition(context.GetWellKnownType(WellKnownType.IntPtr), "total", 0);
            _lengthLocal = new LocalVariableDefinition(context.GetWellKnownType(WellKnownType.IntPtr), "length", 1);

            if (_method.Kind != ArrayMethodKind.Ctor)
            {
                locals.Add(_totalLocal);
                locals.Add(_lengthLocal);
            }
        }

        public static MethodIL? EmitIL(ArrayMethod arrayMethod, IList<LocalVariableDefinition> locals)
        {
            return new ArrayMethodILEmitter(arrayMethod, locals).EmitIL();
        }

        private MethodIL? EmitIL()
        {
            return _method.Kind switch
            {
                ArrayMethodKind.Get or ArrayMethodKind.Set or ArrayMethodKind.AddressWithHiddenArg => EmitILForAccessor(),
                _ => throw new InvalidOperationException(),
            };
        }

        private MethodIL? EmitILForAccessor()
        {
            Debug.Assert(_method.OwningType.IsMdArray);

            var context = _method.Context;
            int pointerSize = context.Target.PointerSize;

            int argStartOffset = _method.Kind == ArrayMethodKind.AddressWithHiddenArg ? 2 : 1;

            ArrayType arrayType = (ArrayType)_method.OwningType;
            var rank = arrayType.Rank;
            var elementType = arrayType.ElementType;

            var helperField = _method.Context.GetWellKnownType(WellKnownType.Object).GetKnownField("m_pEEType");

            var body = new MethodIL() { LocalsCount = 2 };
            uint offset = 0;

            // TODO: ArrayTypeMismatchException checks

            if (rank == 1)
            {
                throw new NotImplementedException("Methods on rank 1 MdArray not yet implemented");
            }

            for (int i = 0; i < rank; i++)
            {
                // First field is EETypePtr, then we have total length, followed by the lengths of each dimension
                int lengthOffset = (2 * pointerSize + i * 2 /* sizeof(nint) */);

                EmitLoadInteriorAddress(helperField, body, ref offset, lengthOffset);

                body.Instructions.Add(new Instruction(ILOpcode.ldind_i, offset++));
                body.Instructions.Add(new Instruction(ILOpcode.stloc, offset++, _lengthLocal));

                body.Instructions.Add(new Instruction(ILOpcode.ldarg, offset++, new ParameterDefinition(_method.OwningType, "", i + argStartOffset)));
                body.Instructions.Add(new Instruction(ILOpcode.conv_i, offset++));

                // TODO: range check

                if (i > 0)
                {
                    body.Instructions.Add(new Instruction(ILOpcode.ldloc, offset++, _totalLocal));
                    body.Instructions.Add(new Instruction(ILOpcode.ldloc, offset++, _lengthLocal));
                    body.Instructions.Add(new Instruction(ILOpcode.mul, offset++));
                    body.Instructions.Add(new Instruction(ILOpcode.add, offset++));
                }

                body.Instructions.Add(new Instruction(ILOpcode.stloc, offset++, _totalLocal));
            }

            // Compute offset of first element in the array
            // First field is EETypePtr, then we have total length, followed by the lengths of each dimension
            int firstElementOffset = (2 * pointerSize + 1 * rank * 2 /* sizeof(nint) */);

            EmitLoadInteriorAddress(helperField, body, ref offset, firstElementOffset);

            body.Instructions.Add(new Instruction(ILOpcode.ldloc, offset++, _totalLocal));
            body.Instructions.Add(new Instruction(ILOpcode.conv_u, offset++));

            int elementSize = elementType.GetElementSize().AsInt;
            if (elementSize != 1)
            {
                body.Instructions.Add(new Instruction(ILOpcode.ldc_i4, offset++, elementSize));
                body.Instructions.Add(new Instruction(ILOpcode.mul, offset++));
            }
            body.Instructions.Add(new Instruction(ILOpcode.add, offset++));

            switch (_method.Kind)
            {
                case ArrayMethodKind.Get:
                    body.Instructions.Add(new Instruction(ILOpcode.ldobj, offset++, elementType));
                    break;

                case ArrayMethodKind.Set:
                    body.Instructions.Add(new Instruction(ILOpcode.ldarg, offset++, new ParameterDefinition(_method.OwningType, "", argStartOffset + rank)));
                    body.Instructions.Add(new Instruction(ILOpcode.stobj, offset++, elementType));
                    break;

                case ArrayMethodKind.Address:
                    break;
            }

            body.Instructions.Add(new Instruction(ILOpcode.ret, offset));
            return body;
        }

        private static void EmitLoadInteriorAddress(FieldDesc helperField, MethodIL body, ref uint offset, int lengthOffset)
        {
            body.Instructions.Add(new Instruction(ILOpcode.ldarg_0, offset++));
            body.Instructions.Add(new Instruction(ILOpcode.ldflda, offset++, helperField));
            body.Instructions.Add(new Instruction(ILOpcode.ldc_i4, offset++, lengthOffset));
            body.Instructions.Add(new Instruction(ILOpcode.add, offset++));
        }
    }
}
