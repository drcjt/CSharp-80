using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Dnlib
{
    public class DnlibInstruction : IL.Instruction
    {
        private readonly DnlibModule _module;
        private readonly dnlib.DotNet.Emit.Instruction _instruction;

        public DnlibInstruction(DnlibModule module, dnlib.DotNet.Emit.Instruction instruction)
            : base(ComputeOpcode(instruction), instruction.Offset)
        {
            _module = module;
            _instruction = instruction;
        }

        private static ILOpcode ComputeOpcode(dnlib.DotNet.Emit.Instruction instruction)
        {
            var code = (int)instruction.OpCode.Code;
            if (code > 0xff)
            {
                code = 0x0100 + (code & 0xff);
            }

            return ((ILOpcode)code);
        }

        public override int GetSize() => _instruction.GetSize();

        public override object Operand
        {
            get
            {
                return _instruction.Operand switch
                {
                    MemberRef memberRef => memberRef.IsFieldRef ? _module.Create((IField)memberRef) : _module.Create((IMethod)memberRef),
                    IField field => _module.Create(@field),
                    IMethod method => _module.Create(method),
                    ITypeDefOrRef typeDefOrRef => _module.Create(typeDefOrRef),
                    dnlib.DotNet.Emit.Instruction[] instructions => instructions.Select(x => new DnlibInstruction(_module, x)).ToArray(),
                    dnlib.DotNet.Emit.Instruction instruction => new DnlibInstruction(_module, instruction),
                    Local local => new LocalVariableDefinition(_module.Create(local.Type), local.Name, local.Index),
                    Parameter parameter => new ParameterDefinition(_module.Create(parameter.Type), parameter.Name, parameter.Index),
                    String s => s,
                    int i => i,
                    sbyte sb => sb,
                    _ => throw new ArgumentException("Cannot get operand of instruction")
                };
            }
        }
    }
}