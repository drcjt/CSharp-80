using ILCompiler.Compiler.Lowerings;
using ILCompiler.Compiler.Ssa;

namespace ILCompiler.Compiler
{
    public class LocalVariableDescriptor
    {
        public bool IsParameter { get; set; }

        public VarType Type { get; set; }

        public int ExactSize { get; set; }

        public int StackOffset { get; set; }

        public bool IsTemp { get; set; }

        public string Name = string.Empty;

        public bool MustInit { get; set; } = false;
        public bool Tracked { get; set; } = false;

        public bool AddressExposed { get; set; } = false;

        public bool InSsa { get; set; } = false;

        public SsaDefList<LocalSsaVariableDescriptor> PerSsaData { get; set; } = new SsaDefList<LocalSsaVariableDescriptor>();

        public LocalSsaVariableDescriptor GetPerSsaData(int ssaNumber) => PerSsaData.SsaDefinition(ssaNumber);
    }
}