using dnlib.DotNet;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ImportContext
    {
        public BasicBlock? CurrentBlock { get; }
        public bool StopImporting { get; set; }
        public MethodDef Method { get; }
        public INameMangler NameMangler { get; }

        public ImportContext(BasicBlock? currentBlock, MethodDef method, INameMangler nameMangler)
        {
            CurrentBlock = currentBlock;
            Method = method;
            NameMangler = nameMangler;
        }
    }
}
