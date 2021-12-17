using ILCompiler.Compiler;
using ILCompiler.Compiler.CodeGenerators;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Importer;
using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ILCompiler.IoC
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; }

        static ServiceProviderFactory()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            services.AddFactory<IMethodCompiler>();
            services.AddTransient<IMethodCompiler, MethodCompiler>();

            services.AddSingleton<CodeGeneratorFactory>();
            services.AddFactory<ICodeGenerator>();
            services.AddTransient<ICodeGenerator, CodeGenerator>();
            services.AddCodeGenerators();

            services.AddFactory<IILImporter>();
            services.AddTransient<IILImporter, ILImporter>();
            services.AddImporters();

            services.AddFactory<ISsaBuilder>();
            services.AddTransient<ISsaBuilder, SsaBuilder>();

            services.AddSingleton<Z80Writer>();

            services.AddSingleton<TypeDependencyAnalyser>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole()).AddTransient<Program>();
            services.AddSingleton<ICompilation, Compilation>();
            services.AddSingleton<IConfiguration, Configuration>();
            services.AddSingleton<INameMangler, NameMangler>();

            services.AddSingleton<IOpcodeImporterFactory, OpcodeImporterFactory>();
            services.AddSingleton<ICodeGeneratorFactory, CodeGeneratorFactory>();
        }
    }
}
