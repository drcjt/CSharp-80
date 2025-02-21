namespace ILCompiler.TypeSystem.Interop
{
    public class PInvokeMetaData
    {
        public readonly string Name;
        public readonly string Module;

        public PInvokeMetaData(string name, string module)
        {
            Name = name;
            Module = module;
        }
    }
}
