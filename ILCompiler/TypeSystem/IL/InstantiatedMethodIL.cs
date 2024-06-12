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
        public override bool OperandIsNotNull => _instruction.OperandIsNotNull;
        public override int GetSize() => _instruction.GetSize();

        public override object GetOperand() => _instruction.GetOperand() switch
        {
            FieldDesc f => f.InstantiateSignature(_typeInstantiation, _methodInstantiation),
            MethodDesc m => m.InstantiateSignature(_typeInstantiation, _methodInstantiation),
            TypeDesc t => t.InstantiateSignature(_typeInstantiation, _methodInstantiation),
            object o => o
        };
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