using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;


namespace ILCompiler.Compiler.CodeGenerators
{
    internal class AllocObjCodeGenerator : ICodeGenerator<AllocObjEntry>
    {
        public void GenerateCode(AllocObjEntry entry, CodeGeneratorContext context)
        {
            // Allocate memory on the heap using simple zero GC/increment a pointer approach
            context.InstructionsBuilder.Pop(BC); // eeType
            //context.InstructionsBuilder.Ld(BC, entry.MangledEETypeName);
            context.InstructionsBuilder.Ld(DE, (ushort)entry.Size);
            context.InstructionsBuilder.Call("NewObject");
            context.InstructionsBuilder.Push(HL);
        }
    }
}
