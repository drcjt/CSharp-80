using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ILCompiler.Compiler
{
    public class ILImporterFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ILImporterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IILImporter GetILImporter()
        {
            return _serviceProvider.GetRequiredService<IILImporter>();
        }
    }
}
