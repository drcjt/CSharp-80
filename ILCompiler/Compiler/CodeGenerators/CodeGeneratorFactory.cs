using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CodeGeneratorFactory : ICodeGeneratorFactory
    {
        public ICodeGenerator<T> GetCodeGenerator<T>() where T : StackEntry
        {
            return ServiceProviderFactory.ServiceProvider.GetRequiredService<ICodeGenerator<T>>();
        }
    }
}
