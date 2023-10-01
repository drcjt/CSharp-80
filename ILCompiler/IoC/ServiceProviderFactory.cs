using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler;
using ILCompiler.Compiler.CodeGenerators;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Importer;
using ILCompiler.Compiler.Lowerings;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Compiler.Z80Assembler;
using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ILCompiler.IoC
{
    public static class ServiceProviderFactory
    {
        private static IServiceProvider? _serviceProvider;
        public static IServiceProvider ServiceProvider 
        { 
            get 
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = CreateServiceProviderFactory();
                }
                return _serviceProvider;
            } 
        }

        public static IServiceProvider CreateServiceProviderFactory()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            services.AddFactory<IMethodCompiler>();
            services.AddTransient<IMethodCompiler, MethodCompiler>();

            services.AddSingleton<IPhaseFactory, PhaseFactory>();

            services.AddTransient<IILImporter, ILImporter>();
            services.AddImporters();

            services.AddTransient<IMorpher, Morpher>();

            services.AddTransient<IRationalizer, Rationalizer>();

            services.AddTransient<IFlowgraph, Flowgraph>();

            services.AddTransient<ISsaBuilder, SsaBuilder>();

            services.AddTransient<ILowering, Lowering>();
            services.AddLowerings();

            services.AddTransient<ICodeGenerator, CodeGenerator>();
            services.AddCodeGenerators();

            services.AddTransient<IObjectAllocator, ObjectAllocator>();

            services.AddSingleton<NativeDependencyAnalyser>();

            services.AddSingleton<Z80Writer>();

            services.AddSingleton<TypeDependencyAnalyser>();

            services.AddSingleton<RTILProvider>();

            services.AddSingleton<CorLibModuleProvider>();

            _serviceProvider = services.BuildServiceProvider();

            return ServiceProvider;
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole()).AddTransient<Program>();
            services.AddSingleton<ICompilation, Compilation>();
            services.AddSingleton<IZ80Assembler, Z80Assembler>();
            services.AddSingleton<IConfiguration, Configuration>();
            services.AddSingleton<INameMangler, NameMangler>();
            services.AddSingleton<PreinitializationManager, PreinitializationManager>();

            services.AddSingleton<ICodeGeneratorFactory, CodeGeneratorFactory>();
            services.AddSingleton<ILoweringFactory, LoweringFactory>();
        }
    }
}
