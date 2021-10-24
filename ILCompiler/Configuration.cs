using ILCompiler.Interfaces;

namespace ILCompiler
{
    public class Configuration : IConfiguration
    {
        public bool DumpIRTrees { get; set; } = false;
        public bool IgnoreUnknownCil { get; set; } = false;
        public bool DontInlineRuntime { get; set; } = false;
        public bool PrintReturnCode { get; set; } = true;
        public string CorelibPath { get; set; } = string.Empty;
        public bool IntegrationTests { get; set; }
    }
}