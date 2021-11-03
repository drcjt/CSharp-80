using ILCompiler.Compiler;
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
            ConfigureImporters(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole()).AddTransient<Program>();
            services.AddSingleton<ICompilation, Compilation>();
            services.AddSingleton<IConfiguration, Configuration>();
            services.AddSingleton<INameMangler, NameMangler>();

            services.AddSingleton<IOpcodeImporterFactory, OpcodeImporterFactory>();
        }

        private static void ConfigureImporters(ServiceCollection services)
        {
            services.AddSingleton<IOpcodeImporter, NopImporter>();
            services.AddSingleton<IOpcodeImporter, LoadIntImporter>();
            services.AddSingleton<IOpcodeImporter, StoreVarImporter>();
            services.AddSingleton<IOpcodeImporter, LoadVarImporter>();
            services.AddSingleton<IOpcodeImporter, AddressOfVarImporter>();
            services.AddSingleton<IOpcodeImporter, StoreIndirectImporter>();
            services.AddSingleton<IOpcodeImporter, LoadIndirectImporter>();
            services.AddSingleton<IOpcodeImporter, StoreFieldImporter>();
            services.AddSingleton<IOpcodeImporter, LoadFieldImporter>();
            services.AddSingleton<IOpcodeImporter, BinaryOperationImporter>();
            services.AddSingleton<IOpcodeImporter, CompareImporter>();
            services.AddSingleton<IOpcodeImporter, BranchImporter>();
            services.AddSingleton<IOpcodeImporter, LoadArgImporter>();
            services.AddSingleton<IOpcodeImporter, StoreArgImporter>();
            services.AddSingleton<IOpcodeImporter, LoadStringImporter>();
            services.AddSingleton<IOpcodeImporter, InitobjImporter>();
            services.AddSingleton<IOpcodeImporter, ConversionImporter>();
            services.AddSingleton<IOpcodeImporter, NegImporter>();
            services.AddSingleton<IOpcodeImporter, RetImporter>();
            services.AddSingleton<IOpcodeImporter, CallImporter>();
            services.AddSingleton<IOpcodeImporter, DupImporter>();
            services.AddSingleton<IOpcodeImporter, NewobjImporter>();
            services.AddSingleton<IOpcodeImporter, PopImporter>();
            services.AddSingleton<IOpcodeImporter, AddressOfFieldImporter>();
            services.AddSingleton<IOpcodeImporter, SwitchImporter>();
            services.AddSingleton<IOpcodeImporter, LocallocImporter>();
        }
    }
}
