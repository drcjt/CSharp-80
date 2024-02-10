using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class NullCheckCodeGenerator : ICodeGenerator<NullCheckEntry>
    {
        public void GenerateCode(NullCheckEntry entry, CodeGeneratorContext context)
        {
            if (context.Configuration.ExceptionSupport && !context.Configuration.SkipNullReferenceCheck)
            {
                var throwHelperMethod = context.CorLibModuleProvider.GetHelperEntryPoint("ThrowHelpers", "ThrowNullReferenceException");
                var mangledThrowHelperMethod = context.NameMangler.GetMangledMethodName(throwHelperMethod);

                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Push(HL);
                context.InstructionsBuilder.Ld(A, H);
                context.InstructionsBuilder.Or(L);
                context.InstructionsBuilder.Call(Emit.Condition.Z, mangledThrowHelperMethod);
            }
        }
    }
}
