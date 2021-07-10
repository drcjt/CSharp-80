namespace ILCompiler.Interfaces
{
    public interface IConfiguration
    {
        public bool IgnoreUnknownCil { get; set; }

        public bool DontInlineRuntime { get; set; }
    }
}
