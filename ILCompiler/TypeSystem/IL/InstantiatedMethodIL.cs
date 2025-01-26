using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;

namespace ILCompiler.TypeSystem.IL
{
    public class InstantiatedInstruction : Instruction
    {
        private readonly Instruction _instruction;
        private readonly Instantiation? _typeInstantiation;
        private readonly Instantiation? _methodInstantiation;

        public InstantiatedInstruction(Instruction instruction, Instantiation? typeInstantiation, Instantiation? methodInstantiation)
            : base(instruction.Opcode, instruction.Offset)
        {
            _instruction = instruction;
            _typeInstantiation = typeInstantiation;
            _methodInstantiation = methodInstantiation;
        }
        public override int GetSize() => _instruction.GetSize();

        public override object Operand
        {
            get
            {
                return _instruction.Operand switch
                {
                    FieldDesc f => f.InstantiateSignature(_typeInstantiation, _methodInstantiation),
                    MethodDesc m => m.InstantiateSignature(_typeInstantiation, _methodInstantiation),
                    TypeDesc t => t.InstantiateSignature(_typeInstantiation, _methodInstantiation),
                    object o => o
                };
            }
        }
    }

    public class InstantiatedMethodIL : MethodIL
    {
        private readonly MethodIL _methodIL;

        private readonly List<Instruction> _instantiatedInstructions = new List<Instruction>();
        public InstantiatedMethodIL(MethodDesc owningMethod, MethodIL methodIL)
        {
            var typeInstantiation = owningMethod.OwningType.Instantiation;
            var methodInstantiation = owningMethod.Instantiation;

            var context = owningMethod.Context;
            var dnlibModule = (DnlibModule)context.SystemModule!;
            var ilProvider = dnlibModule.ILProvider;

            _methodIL = ilProvider.GetMethodIL(owningMethod, dnlibModule) ?? methodIL;

            foreach (var instruction in _methodIL.Instructions)
            {
                _instantiatedInstructions.Add(new InstantiatedInstruction(instruction, typeInstantiation, methodInstantiation));
            }
        }

        public override IList<Instruction> Instructions => _instantiatedInstructions;
        public override ILExceptionRegion[] GetExceptionRegions() => _methodIL.GetExceptionRegions();
        public override int LocalsCount => _methodIL.LocalsCount;

        public override bool IsInitLocals => _methodIL.IsInitLocals;

        public override MethodIL GetMethodILDefinition() => _methodIL;
    }
}