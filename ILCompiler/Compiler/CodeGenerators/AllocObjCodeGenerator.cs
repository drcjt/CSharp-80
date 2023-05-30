using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;


namespace ILCompiler.Compiler.CodeGenerators
{
    internal class AllocObjCodeGenerator : ICodeGenerator<AllocObjEntry>
    {
        public void GenerateCode(AllocObjEntry entry, CodeGeneratorContext context)
        {
            // Allocate memory on the heap using simple zero GC/increment a pointer approach
            context.Emitter.Ld(BC, entry.MangledEETypeName);
            context.Emitter.Ld(DE, (ushort)entry.Size);
            context.Emitter.Call("NewObject");
        }
    }
}
