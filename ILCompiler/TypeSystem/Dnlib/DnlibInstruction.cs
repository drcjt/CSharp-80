using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Dnlib
{
    public class DnlibInstruction : Instruction
    {
        private readonly DnlibModule _module;
        private readonly dnlib.DotNet.Emit.Instruction _instruction;

        public DnlibInstruction(DnlibModule module, dnlib.DotNet.Emit.Instruction instruction)
        {
            _module = module;
            _instruction = instruction;
        }

        public override ILOpcode Opcode
        {
            get
            {
                var code = (int)_instruction.OpCode.Code;
                if (code > 0xff)
                {
                    code = 0x0100 + (code & 0xff);
                }

                return ((ILOpcode)code);
            }
        }

        public override uint Offset => _instruction.Offset;

        public override int GetSize() => _instruction.GetSize();

        public override object GetOperandAs<T>()
        {
            if (typeof(T) == typeof(FieldDesc))
                return _module.Create(_instruction.OperandAs<IField>());

            if (typeof(T) == typeof(MethodDesc))
                return _module.Create(_instruction.OperandAs<IMethod>());

            if (typeof(T) == typeof(TypeDesc))
                return _module.Create(_instruction.OperandAs<ITypeDefOrRef>());

            if (typeof(T) == typeof(string))
                return _instruction.OperandAs<String>();

            if (typeof(T) == typeof(Instruction[]))
            {
                if (_instruction.Operand is not dnlib.DotNet.Emit.Instruction[] instructions) return Array.Empty<Instruction>();
                return instructions.Select(x => new DnlibInstruction(_module, x)).ToArray();
            }

            if (typeof(T) == typeof(Instruction))
            {
                return new DnlibInstruction(_module, _instruction.OperandAs<dnlib.DotNet.Emit.Instruction>());
            }

            if (typeof(T) == typeof(int))
                return _instruction.OperandAs<int>();
            if (typeof(T) == typeof(sbyte))
                return _instruction.OperandAs<sbyte>();

            if (typeof(T) == typeof(LocalVariableDefinition))
            {
                var local = _instruction.OperandAs<dnlib.DotNet.Emit.Local>();
                return new LocalVariableDefinition(_module.Create(local.Type), local.Name, local.Index);
            }

            if (typeof(T) == typeof(ParameterDefinition))
            {
                var parameter = _instruction.OperandAs<Parameter>();
                return new ParameterDefinition(_module.Create(parameter.Type), parameter.Name, parameter.Index);
            }

            throw new ArgumentException("Cannot get operand of instruction as type {T}");
        }
    }
}