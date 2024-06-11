using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.PreInit
{
    internal class TypePreinitializer
    {
        private readonly TypeDesc _type;
        private readonly IList<Instruction> _instructions;
        private readonly IDictionary<FieldDesc, Value> _fieldValues = new Dictionary<FieldDesc, Value>();
        private readonly DnlibModule _module;
        public TypePreinitializer(TypeDesc type, IList<Instruction> instructions, DnlibModule module)
        {
            _type = type;
            _instructions = instructions;
            _module = module;

            foreach (var field in type.GetFields())
            {
                if (field.IsStatic && !field.IsLiteral)
                {
                    _fieldValues.Add(field, new ValueTypeValue(field.FieldType));
                }
            }
        }

        public static PreinitializationInfo ScanType(TypeDesc type, DnlibModule module)
        {
            var cctor = type.GetStaticConstructor();
            var instructions = cctor!.MethodIL!.Instructions;

            var typePreinitializer = new TypePreinitializer(type, instructions, module);
            var status = typePreinitializer.TryScanMethod();

            if (status)
            {
                var values = new List<KeyValuePair<FieldDesc, ISerializableValue>>();
                foreach (var kvp in typePreinitializer._fieldValues)
                {
                    values.Add(new KeyValuePair<FieldDesc, ISerializableValue>(kvp.Key, kvp.Value));
                }
                return new PreinitializationInfo(values);
            }
            else
            {
                return new PreinitializationInfo();
            }
        }

        private bool TryScanMethod()
        {
            var stack = new Stack();
            var instructionCounter = 0;

            while (instructionCounter < _instructions.Count)
            {
                var instruction = _instructions[instructionCounter];
                var opcode = instruction.Opcode;
                switch (opcode)
                {
                    case ILOpcode.ldc_i4_m1:
                    case ILOpcode.ldc_i4_s:
                    case ILOpcode.ldc_i4:
                    case ILOpcode.ldc_i4_0:
                    case ILOpcode.ldc_i4_1:
                    case ILOpcode.ldc_i4_2:
                    case ILOpcode.ldc_i4_3:
                    case ILOpcode.ldc_i4_4:
                    case ILOpcode.ldc_i4_5:
                    case ILOpcode.ldc_i4_6:
                    case ILOpcode.ldc_i4_7:
                    case ILOpcode.ldc_i4_8:
                        {
                            int value = opcode switch
                            {
                                ILOpcode.ldc_i4_m1 => -1,
                                ILOpcode.ldc_i4_s => (sbyte)instruction.GetOperand(),
                                ILOpcode.ldc_i4 => (int)instruction.GetOperand(),
                                _ => opcode - ILOpcode.ldc_i4_0,
                            };
                            stack.Push(StackValueKind.Int32, ValueTypeValue.FromInt32(value));
                        }
                        break;

                    case ILOpcode.stsfld:
                        {
                            var fieldDesc = (FieldDesc)instruction.GetOperand();

                            if (!fieldDesc.IsStatic || fieldDesc.IsLiteral)
                            {
                                throw new InvalidProgramException();
                            }

                            if (fieldDesc.OwningType != _type)
                            {
                                // Store into other static
                                return false;
                            }

                            var value = stack.PopIntoLocation(fieldDesc.FieldType);
                            _fieldValues[fieldDesc] = value;
                        }
                        break;

                    case ILOpcode.ret:
                        return true;

                    default:
                        return false;
                }

                instructionCounter++;
            }

            return false;
        }
    }
}