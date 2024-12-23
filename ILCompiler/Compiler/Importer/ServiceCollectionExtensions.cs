using Microsoft.Extensions.DependencyInjection;

namespace ILCompiler.Compiler.Importer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImporters(this IServiceCollection services)
        {
            services.AddSingleton<IOpcodeImporter, ConstrainedImporter>();
            services.AddSingleton<IOpcodeImporter, UnboxImporter>();
            services.AddSingleton<IOpcodeImporter, BoxImporter>();
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
            services.AddSingleton<IOpcodeImporter, InitblkImporter>();
            services.AddSingleton<IOpcodeImporter, CpblkImporter>();
            services.AddSingleton<IOpcodeImporter, ConversionImporter>();
            services.AddSingleton<IOpcodeImporter, UnaryOperationImporter>();
            services.AddSingleton<IOpcodeImporter, RetImporter>();
            services.AddSingleton<IOpcodeImporter, CallImporter>();
            services.AddSingleton<IOpcodeImporter, DupImporter>();
            services.AddSingleton<IOpcodeImporter, NewobjImporter>();
            services.AddSingleton<IOpcodeImporter, PopImporter>();
            services.AddSingleton<IOpcodeImporter, AddressOfFieldImporter>();
            services.AddSingleton<IOpcodeImporter, SwitchImporter>();
            services.AddSingleton<IOpcodeImporter, LocallocImporter>();
            services.AddSingleton<IOpcodeImporter, ShiftOperationImporter>();
            services.AddSingleton<IOpcodeImporter, NewarrImporter>();
            services.AddSingleton<IOpcodeImporter, LoadElemImporter>();
            services.AddSingleton<IOpcodeImporter, StoreElemImporter>();
            services.AddSingleton<IOpcodeImporter, LoadLengthImporter>();
            services.AddSingleton<IOpcodeImporter, LoadArgAddressImporter>();
            services.AddSingleton<IOpcodeImporter, LoadNullImporter>();
            services.AddSingleton<IOpcodeImporter, SizeOfImporter>();
            services.AddSingleton<IOpcodeImporter, LoadElemAddressImporter>();
            services.AddSingleton<IOpcodeImporter, IsInstImporter>();
            services.AddSingleton<IOpcodeImporter, ThrowImporter>();
            services.AddSingleton<IOpcodeImporter, LeaveImporter>();

            return services;
        }
    }
}
