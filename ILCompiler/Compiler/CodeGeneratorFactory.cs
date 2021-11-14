using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ILCompiler.Compiler
{
    // TODO: Consider creating a generic "phase" factory & merging
    // this code with that in ILImporterFactory

    // TODO: Consider implementing generic "Factories" as per https://espressocoder.com/2018/10/08/injecting-a-factory-service-in-asp-net-core/
    public class CodeGeneratorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public CodeGeneratorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICodeGenerator GetCodeGenerator()
        {
            return _serviceProvider.GetRequiredService<ICodeGenerator>();
        }
    }
}
