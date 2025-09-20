using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.Inlining
{
    public record InlineLocalVariableInfo
    {
        public int TempNumber { get; set; }
        public bool HasTemp { get; set; }
        public TypeDesc? Type { get; set; }
    }
}
