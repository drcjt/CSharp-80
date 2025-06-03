using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Compiler;
using ILCompiler.Compiler.CodeGenerators;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Lowerings;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Compiler.Z80Assembler;
using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ILCompiler.IL;
using ILCompiler.Compiler.Peephole;
using ILCompiler.Compiler.OpcodeImporters;

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

            services.AddSingleton<Optimizer>();

            services.AddSingleton<IPhaseFactory, PhaseFactory>();

            services.AddTransient<IImporter, Importer>();
            services.AddImporters();

            services.AddTransient<IMorpher, Morpher>();

            services.AddTransient<IInliner, Inliner>();

            services.AddTransient<ILoopFinder, LoopFinder>();

            services.AddTransient<IRationalizer, Rationalizer>();

            services.AddTransient<IEarlyValuePropagation, EarlyValuePropagation>();

            services.AddTransient<IFlowgraph, Flowgraph>();

            services.AddTransient<ISsaBuilder, SsaBuilder>();

            services.AddTransient<ILowering, Lowering>();
            services.AddLowerings();

            services.AddTransient<ICodeGenerator, CodeGenerator>();
            services.AddCodeGenerators();

            services.AddTransient<IObjectAllocator, ObjectAllocator>();

            services.AddSingleton<NodeFactory>();

            services.AddSingleton<NativeDependencyAnalyser>();

            services.AddSingleton<Z80AssemblyWriter>();

            services.AddSingleton<DependencyAnalyzer>();

            services.AddSingleton<RTILProvider>();

            services.AddSingleton<CorLibModuleProvider>();

            services.AddSingleton<TypeSystemContext>();
            services.AddSingleton<DnlibModule>();

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
