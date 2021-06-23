using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler
{
    public class LocalVariableDescriptor
    {
        public bool IsParameter { get; set; }

        public StackValueKind Kind { get; set; }
    }
}
