using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.Compiler
{
    public class PhaseFactory : IPhaseFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public PhaseFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>() where T : IPhase
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
