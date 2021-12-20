using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.Compiler.Lowerings
{
    public class LoweringFactory : ILoweringFactory
    {
        public ILowering<T>? GetLowering<T>() where T : StackEntry
        {
            return ServiceProviderFactory.ServiceProvider.GetService<ILowering<T>>();
        }
    }
}
