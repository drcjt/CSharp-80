using ILCompiler.Compiler;
using System.CommandLine;

namespace ILCompiler
{
    internal class ConfigurationOptions
    {
        public readonly Option<bool> IgnoreUnknownCilOption = new("IgnoreUnknownCil", "-f", "--ignoreUnknownCil");
        public readonly Option<bool> PrintReturnCodeOption = new("PrintReturnCode", "-r", "--printReturnCode");
        public readonly Option<string> CoreLibPathOption = new("CorelibPath", "-cl", "--corelibPath");
        public readonly Option<bool> IntegrationTestsOption = new("CompileForIntegrationTests", "-it", "--integrationTests");
        public readonly Option<bool> DumpIRTreesOption = new("DumpIRTrees", "-dir", "--dumpIRTrees");
        public readonly Option<bool> DumpSsaOption = new("DumpSsaInformation", "-dssa", "--dumpSsa");
        public readonly Option<bool> DumpFlowGraphs = new("DumpFlowGraphs", "-dfg", "--dumpFlowGraphs");

        public readonly Option<string?> RuntimeIdentifier = new("RuntimeIdentifier", "-rid", "--runtimeIdentifier");
        public readonly Option<TargetArchitecture> TargetArchitectureOption = new("TargetArchitecture", "-a", "--targetArchitecture");
        public readonly Option<int?> StackStartOption = new("StackStartAddress", "-ss", "--stackStart");
        public readonly Option<string> AssemblerArguments = new("AssemblerArguments", "-aa", "--assemblerArguments");
        public readonly Option<string> AssemblerOutput = new("AssemblerOutputType", "-ao", "--assemblerOutput");
        public readonly Option<bool> NoListFile = new("NoListFile", "-nl", "--noListFile");
        public readonly Option<bool> SkipArrayBoundsCheck = new("NoArrayBoundsCheck", "-nb", "--noBoundsCheck");
        public readonly Option<bool> SkipNullReferenceCheck = new("NoNullReferenceCheck", "-nn", "--noNullCheck");
        public readonly Option<bool> Optimize = new("EnableOptimizations", "-O", "--optimize");

        public void AddToCommand(Command command)
        {
            command.Options.Add(IgnoreUnknownCilOption);
            command.Options.Add(PrintReturnCodeOption);
            command.Options.Add(CoreLibPathOption);
            command.Options.Add(IntegrationTestsOption);
            command.Options.Add(DumpIRTreesOption);
            command.Options.Add(RuntimeIdentifier);
            command.Options.Add(TargetArchitectureOption);
            command.Options.Add(StackStartOption);
            command.Options.Add(AssemblerArguments);
            command.Options.Add(AssemblerOutput);
            command.Options.Add(NoListFile);
            command.Options.Add(SkipArrayBoundsCheck);
            command.Options.Add(SkipNullReferenceCheck);
            command.Options.Add(Optimize);
        }
    }
}
