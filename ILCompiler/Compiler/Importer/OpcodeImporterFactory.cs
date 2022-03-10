using dnlib.DotNet.Emit;

namespace ILCompiler.Compiler.Importer
{
    public interface IOpcodeImporterFactory
    {
        IOpcodeImporter? GetImporter(Code opcode);
    }

    public class OpcodeImporterFactory : IOpcodeImporterFactory
    {
        private IEnumerable<IOpcodeImporter> _importers;

        public OpcodeImporterFactory(IEnumerable<IOpcodeImporter> importers)
        {
            _importers = importers;
        }

        public IOpcodeImporter? GetImporter(Code opcode)
        {
            return _importers.FirstOrDefault(importer => importer.CanImport(opcode));
        }
    }
}
