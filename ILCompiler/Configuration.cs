using ILCompiler.Interfaces;

namespace ILCompiler
{
    public class Configuration : IConfiguration
    {
        public bool IgnoreUnknownCil { get; set; } = false;

        public bool DontInlineRuntime { get; set; } = false;

        public bool PrintReturnCode { get; set; } = true;
    }
}
