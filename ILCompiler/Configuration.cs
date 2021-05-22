using ILCompiler.Interfaces;

namespace ILCompiler
{
    public class Configuration : IConfiguration
    {
        public bool IgnoreUnknownCil { get; set; } = false;
    }
}
