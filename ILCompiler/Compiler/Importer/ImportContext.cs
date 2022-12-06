using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ImportContext
    {
        public BasicBlock CurrentBlock { get; }
        public BasicBlock? FallThroughBlock { get; }
        public bool StopImporting { get; set; }
        public MethodDesc Method { get; }
        public INameMangler NameMangler { get; }

        public ImportContext(BasicBlock currentBlock, BasicBlock? fallthroughBlock, MethodDesc method, INameMangler nameMangler)
        {
            CurrentBlock = currentBlock;
            FallThroughBlock = fallthroughBlock;
            Method = method;
            NameMangler = nameMangler;
        }
    }
}
