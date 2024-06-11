using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.IL
{
    public class InstantiatedInstruction : Instruction
    {
        private readonly Instruction _instruction;
        private readonly Instantiation? _typeInstantiation;
        private readonly Instantiation? _methodInstantiation;

        public InstantiatedInstruction(Instruction instruction, Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            _instruction = instruction;
            _typeInstantiation = typeInstantiation;
            _methodInstantiation = methodInstantiation;
        }
        public override ILOpcode Opcode => _instruction.Opcode;
        public override uint Offset => _instruction.Offset;
        public override int GetSize() => _instruction.GetSize();

        public override object GetOperandAs<T>()
        {
            if (typeof(T) == typeof(FieldDesc))
            {
                var fieldDesc = (FieldDesc)_instruction.GetOperandAs<FieldDesc>();
                return fieldDesc.InstantiateSignature(_typeInstantiation, _methodInstantiation);

            }
            if (typeof(T) == typeof(MethodDesc))
            {
                var methodDesc = (MethodDesc)_instruction.GetOperandAs<MethodDesc>();
                return methodDesc.InstantiateSignature(_typeInstantiation, _methodInstantiation);
            }
            if (typeof(T) == typeof(TypeDesc))
            {
                var typeDesc = (TypeDesc)_instruction.GetOperandAs<TypeDesc>();
                return typeDesc.InstantiateSignature(_typeInstantiation, _methodInstantiation);
            }

            return _instruction.GetOperandAs<T>();
        }
    }

    public class InstantiatedMethodIL : MethodIL
    {
        private readonly MethodIL _methodIL;
        private readonly MethodDesc _owningMethod;
        private readonly Instantiation? _typeInstantiation;
        private readonly Instantiation _methodInstantiation;

        private readonly List<Instruction> _instantiatedInstructions = new List<Instruction>();
        public InstantiatedMethodIL(MethodDesc owningMethod, MethodIL methodIL)
        {
            _methodIL = methodIL;
            _owningMethod = owningMethod;
            _typeInstantiation = owningMethod.OwningType.Instantiation;
            _methodInstantiation = owningMethod.Instantiation;

            foreach (var instruction in _methodIL.Instructions)
            {
                _instantiatedInstructions.Add(new InstantiatedInstruction(instruction, _typeInstantiation, _methodInstantiation));
            }
        }

        public override IList<Instruction> Instructions => _instantiatedInstructions;
        public override ILExceptionRegion[] GetExceptionRegions() => _methodIL.GetExceptionRegions();
        public override int LocalsCount => _methodIL.LocalsCount;

        public override bool IsInitLocals => _methodIL.IsInitLocals;
    }
}