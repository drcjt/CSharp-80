using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class AllocObjCodeGenerator : ICodeGenerator<AllocObjEntry>
    {
        public void GenerateCode(AllocObjEntry entry, CodeGeneratorContext context)
        {
            // Allocate memory on the heap using simple zero GC/increment a pointer approach

            context.Emitter.Ld(R16.HL, (short)entry.Size);
            context.Emitter.Push(R16.HL);
            context.Emitter.Call("NewObject");
        }
    }
}
