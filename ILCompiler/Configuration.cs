using ILCompiler.Compiler;
using ILCompiler.Interfaces;

namespace ILCompiler
{
    public class Configuration : IConfiguration
    {
        public bool DumpIRTrees { get; set; } = false;
        public bool DumpSsa { get; set; } = false;
        public bool DumpFlowGraphs {  get; set; } = false;
        public bool IgnoreUnknownCil { get; set; } = false;
        public bool PrintReturnCode { get; set; } = true;
        public string CorelibPath { get; set; } = string.Empty;
        public bool IntegrationTests { get; set; }
        public TargetArchitecture TargetArchitecture { get; set; }
        public int? StackStart { get; set; }
        public string AssemblerArguments { get; set; } = string.Empty;
        public string AssemblerOutput { get; set; } = string.Empty;
        public bool NoListFile { get; set; } = true;
        public bool SkipArrayBoundsCheck { get; set; } = false;
        public bool SkipNullReferenceCheck { get; set; } = false;
    }
}