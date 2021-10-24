using System.Collections.Generic;

namespace ILCompiler.Compiler.Importer
{
    public class OpcodeImporterFactory
    {
        private static IList<IOpcodeImporter> _opcodeImporters = new List<IOpcodeImporter>();
        public static IList<IOpcodeImporter> GetAllOpcodeImporters()
        {
            if (_opcodeImporters.Count == 0)
            {
                _opcodeImporters.Add(new NopImporter());
                _opcodeImporters.Add(new LoadIntImporter());
                _opcodeImporters.Add(new StoreVarImporter());
                _opcodeImporters.Add(new LoadVarImporter());
                _opcodeImporters.Add(new AddressOfVarImporter());
                _opcodeImporters.Add(new StoreIndirectImporter());
                _opcodeImporters.Add(new LoadIndirectImporter());
                _opcodeImporters.Add(new StoreFieldImporter());
                _opcodeImporters.Add(new LoadFieldImporter());
                _opcodeImporters.Add(new BinaryOperationImporter());
                _opcodeImporters.Add(new CompareImporter());
                _opcodeImporters.Add(new BranchImporter());
                _opcodeImporters.Add(new LoadArgImporter());
                _opcodeImporters.Add(new StoreArgImporter());
                _opcodeImporters.Add(new LoadStringImporter());
                _opcodeImporters.Add(new InitobjImporter());
                _opcodeImporters.Add(new ConversionImporter());
                _opcodeImporters.Add(new NegImporter());
                _opcodeImporters.Add(new RetImporter());
                _opcodeImporters.Add(new CallImporter());
                _opcodeImporters.Add(new DupImporter());
                _opcodeImporters.Add(new NewobjImporter());
                _opcodeImporters.Add(new PopImporter());
                _opcodeImporters.Add(new AddressOfFieldImporter());
                _opcodeImporters.Add(new SwitchImporter());
                _opcodeImporters.Add(new LocallocImporter());
            }
            return _opcodeImporters;
        }
    }
}
