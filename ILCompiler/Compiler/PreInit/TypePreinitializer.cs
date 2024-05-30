using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using Instruction = dnlib.DotNet.Emit.Instruction;

namespace ILCompiler.Compiler.PreInit
{
    internal class TypePreinitializer
    {
        private readonly TypeDesc _type;
        private readonly IList<Instruction> _instructions;
        private readonly IDictionary<FieldDesc, Value> _fieldValues = new Dictionary<FieldDesc, Value>();
        public TypePreinitializer(TypeDesc type, IList<Instruction> instructions)
        {
            _type = type;
            _instructions = instructions;

            foreach (var field in type.GetFields())
            {
                if (field.IsStatic && !field.IsLiteral)
                {
                    _fieldValues.Add(field, new ValueTypeValue(field.FieldType));
                }
            }
        }

        public static PreinitializationInfo ScanType(TypeDesc type)
        {
            var cctor = type.GetStaticConstructor();
            var instructions = cctor!.Body.Instructions;

            var typePreinitializer = new TypePreinitializer(type, instructions);
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
                var opcode = instruction.OpCode.Code;
                switch (opcode)
                {
                    case Code.Ldc_I4_M1:
                    case Code.Ldc_I4_S:
                    case Code.Ldc_I4:
                    case Code.Ldc_I4_0:
                    case Code.Ldc_I4_1:
                    case Code.Ldc_I4_2:
                    case Code.Ldc_I4_3:
                    case Code.Ldc_I4_4:
                    case Code.Ldc_I4_5:
                    case Code.Ldc_I4_6:
                    case Code.Ldc_I4_7:
                    case Code.Ldc_I4_8:
                        {
                            int value = opcode switch
                            {
                                Code.Ldc_I4_M1 => -1,
                                Code.Ldc_I4_S => (sbyte)instruction.Operand,
                                Code.Ldc_I4 => (int)instruction.Operand,
                                _ => opcode - Code.Ldc_I4_0,
                            };
                            stack.Push(StackValueKind.Int32, ValueTypeValue.FromInt32(value));
                        }
                        break;

                    case Code.Stsfld:
                        {
                            var fieldDefOrRef = (IField)instruction.Operand;
                            var fieldDesc = _type.Context.Create(fieldDefOrRef);

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

                    case Code.Ret:
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
