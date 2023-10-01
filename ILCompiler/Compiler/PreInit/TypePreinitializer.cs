using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Instruction = dnlib.DotNet.Emit.Instruction;

namespace ILCompiler.Compiler.PreInit
{
    internal class TypePreinitializer
    {
        private readonly TypeDef _type;
        private readonly IList<Instruction> _instructions;
        private readonly IDictionary<FieldDef, Value> _fieldValues = new Dictionary<FieldDef, Value>();
        public TypePreinitializer(TypeDef type, IList<Instruction> instructions)
        {
            _type = type;
            _instructions = instructions;

            foreach (var field in type.Fields)
            {
                if (field.IsStatic && !field.IsLiteral)
                {
                    _fieldValues.Add(field, new ValueTypeValue(field.FieldSig.GetFieldType()));
                }
            }
        }

        public static PreinitializationInfo ScanType(TypeDef type)
        {
            var cctor = type.FindStaticConstructor();
            var instructions = cctor.Body.Instructions;

            var typePreinitializer = new TypePreinitializer(type, instructions);
            var status = typePreinitializer.TryScanMethod();

            if (status)
            {
                var values = new List<KeyValuePair<FieldDef, ISerializableValue>>();
                foreach (var kvp in typePreinitializer._fieldValues)
                {
                    values.Add(new KeyValuePair<FieldDef, ISerializableValue>(kvp.Key, kvp.Value));
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
                            var fieldDefOrRef = instruction.Operand as IField;
                            var fieldDef = fieldDefOrRef.ResolveFieldDefThrow();

                            if (!fieldDef.IsStatic || fieldDef.IsLiteral)
                            {
                                throw new InvalidProgramException();
                            }

                            if (fieldDef.DeclaringType != _type)
                            {
                                // Store into other static
                                return false;
                            }

                            var value = stack.PopIntoLocation(fieldDef.FieldType);
                            _fieldValues[fieldDef] = value;
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
