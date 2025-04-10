using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.IL.Stubs
{
    public class ILCodeStream
    {
        internal readonly List<Instruction> instructions = [];
        private readonly List<(Instruction, ILCodeLabel)> _instructionsNeedingPatching = [];

        public void Emit(ILOpcode opcode, object? operand = null)
        {
            instructions.Add(new Instruction(opcode, operand));
        }

        public void Emit(ILOpcode opcode, ILCodeLabel label)
        {
            var instruction = new Instruction(opcode);
            instructions.Add(instruction);
            _instructionsNeedingPatching.Add((instruction, label));
        }

        public void EmitLdArg(ParameterDefinition parameter) => Emit(ILOpcode.ldarg, parameter);
        public void EmitLdFlda(FieldDesc field) => Emit(ILOpcode.ldflda, field);
        public void EmitLdcI4(int value) => Emit(ILOpcode.ldc_i4, value);
        public void EmitStLoc(LocalVariableDefinition localVariable) => Emit(ILOpcode.stloc, localVariable);
        public void EmitLdLoc(LocalVariableDefinition localVariable) => Emit(ILOpcode.ldloc, localVariable);

        public void EmitLabel(ILCodeLabel label)
        {
            label.Place(this, instructions.Count);
        }

        public void PatchLabels()
        {
            foreach (var (instruction, label) in _instructionsNeedingPatching)
            {
                var targetInstruction = instructions[label.Offset];

                instruction.Operand = targetInstruction;
            }   
        }
    }

    public class ILCodeLabel
    {
        private ILCodeStream? _codeStream;
        private int _offsetWithinCodeStream;

        public int Offset => _codeStream == null ? -1 : _offsetWithinCodeStream;

        internal void Place(ILCodeStream codeStream, int offsetWithinCodeStream)
        {
            _codeStream = codeStream;
            _offsetWithinCodeStream = offsetWithinCodeStream;
        }
    }

    internal class ILEmitter
    {
        private readonly List<ILCodeStream> _codeStreams = [];
        private readonly List<LocalVariableDefinition> _locals = [];

        public ILCodeStream NewCodeStream()
        {
            var codeStream = new ILCodeStream();
            _codeStreams.Add(codeStream);
            return codeStream;
        }

        public LocalVariableDefinition NewLocal(TypeDesc type, string name)
        {
            var local = new LocalVariableDefinition(type, name, _locals.Count);
            _locals.Add(local);
            return local;
        }

        public static ILCodeLabel NewCodeLabel() => new();

        public MethodIL Link()
        {
            var allInstructions = new List<Instruction>();
            uint offset = 0;
            foreach (var codeStream in _codeStreams)
            {
                foreach (var instruction in codeStream.instructions)
                {
                    instruction.Offset = offset++;
                    allInstructions.Add(instruction);
                }
            }

            foreach (var codeStream in _codeStreams)
            {
                codeStream.PatchLabels();
            }

            var methodIL = new MethodIL()
            {
                Instructions = allInstructions,
                Locals = _locals,
                LocalsCount = _locals.Count,
            };
            return methodIL;
        }
    }
}
