using ILCompiler.Interfaces;
using System.CommandLine.Binding;

namespace ILCompiler
{
    internal class ConfigurationBinder : BinderBase<IConfiguration>
    {
        private readonly ConfigurationOptions _configurationOptions;

        public ConfigurationBinder(ConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }

        protected override IConfiguration GetBoundValue(BindingContext bindingContext)
        {
            return new Configuration
            {
                IgnoreUnknownCil = bindingContext.ParseResult.GetValueForOption(_configurationOptions.IgnoreUnknownCilOption),
                DontInlineRuntime = bindingContext.ParseResult.GetValueForOption(_configurationOptions.DontInlineRuntimeOption),
                PrintReturnCode = bindingContext.ParseResult.GetValueForOption(_configurationOptions.PrintReturnCodeOption),
                CorelibPath = bindingContext.ParseResult.GetValueForOption(_configurationOptions.CoreLibPathOption) ?? "",
                IntegrationTests = bindingContext.ParseResult.GetValueForOption(_configurationOptions.IntegrationTestsOption),
                DumpIRTrees = bindingContext.ParseResult.GetValueForOption(_configurationOptions.DumpIRTreesOption),
                TargetArchitecture = bindingContext.ParseResult.GetValueForOption(_configurationOptions.TargetArchitectureOption),
                StackStart = bindingContext.ParseResult.GetValueForOption(_configurationOptions.StackStartOption),
                AssemblerArguments = bindingContext.ParseResult.GetValueForOption(_configurationOptions.AssemblerArguments) ?? "",
                AssemblerOutput = bindingContext.ParseResult.GetValueForOption(_configurationOptions.AssemblerOutput) ?? "",
                NoListFile = bindingContext.ParseResult.GetValueForOption(_configurationOptions.NoListFile),
            };
        }
    }
}
