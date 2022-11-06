using ILCompiler.Compiler;
using System.CommandLine;

namespace ILCompiler
{
    internal class ConfigurationOptions
    {
        public readonly Option<bool> IgnoreUnknownCilOption = new(new[] { "-f", "--ignoreUnknownCil" }, "Ignore unknown cil");
        public readonly Option<bool> DontInlineRuntimeOption = new(new[] { "-i", "--dontInlineRuntime" }, "Don't inline runtime assembly");
        public readonly Option<bool> PrintReturnCodeOption = new(new[] { "-r", "--printReturnCode" }, "Print return code");
        public readonly Option<string> CoreLibPathOption = new(new[] { "-cl", "--corelibPath" }, "Core lib path");
        public readonly Option<bool> IntegrationTestsOption = new(new[] { "-it", "--integrationTests" }, "Compile for integration tests");
        public readonly Option<bool> DumpIRTreesOption = new(new[] { "-d", "--dumpIRTrees" }, "Dump IR trees");
        public readonly Option<TargetArchitecture> TargetArchitectureOption = new(new[] { "-a", "--targetArchitecture" }, getDefaultValue : () => TargetArchitecture.TRS80, "Target Architecture");
        public readonly Option<int> StackStartOption = new(new[] { "-ss", "--stackStart" }, getDefaultValue: () => 0x7fff, "Stack Start Address");

        public void AddToCommand(Command command)
        {
            command.AddOption(IgnoreUnknownCilOption);
            command.AddOption(DontInlineRuntimeOption);
            command.AddOption(PrintReturnCodeOption);
            command.AddOption(CoreLibPathOption);
            command.AddOption(IntegrationTestsOption);
            command.AddOption(DumpIRTreesOption);
            command.AddOption(TargetArchitectureOption);
            command.AddOption(StackStartOption);
        }
    }
}
