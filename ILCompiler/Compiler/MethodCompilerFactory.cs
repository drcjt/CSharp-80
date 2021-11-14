using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ILCompiler.Compiler
{
    public class MethodCompilerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public MethodCompilerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMethodCompiler GetMethodCompiler()
        {
            return _serviceProvider.GetRequiredService<IMethodCompiler>();
        }
    }
}
