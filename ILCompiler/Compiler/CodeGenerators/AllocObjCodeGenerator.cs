using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class AllocObjCodeGenerator : ICodeGenerator<AllocObjEntry>
    {
        public void GenerateCode(AllocObjEntry entry, CodeGeneratorContext context)
        {
            // Allocate memory on the heap using simple zero GC/increment a pointer approach
            context.InstructionsBuilder.Pop(HL); // eeType
            context.InstructionsBuilder.Call("NewObjectNoSize");
        }
    }
}
