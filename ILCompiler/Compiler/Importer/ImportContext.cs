using dnlib.DotNet;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ImportContext
    {
        public BasicBlock CurrentBlock { get; set; }
        public bool StopImporting { get; set; }
        public MethodDef Method { get; set; }
        public INameMangler NameMangler { get; set; }
    }
}
