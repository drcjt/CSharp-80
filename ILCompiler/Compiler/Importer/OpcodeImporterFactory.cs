using System.Collections.Generic;

namespace ILCompiler.Compiler.Importer
{
    public class OpcodeImporterFactory
    {
        private static IList<IOpcodeImporter> _opcodeImporters;

        public static IList<IOpcodeImporter> GetAllOpcodeImporters()
        {
            if (_opcodeImporters == null)
            {
                _opcodeImporters = new List<IOpcodeImporter>()
                {
                    new NopImporter(),
                    new LoadIntImporter(),
                    new StoreVarImporter(),
                    new LoadVarImporter(),
                    new AddressOfVarImporter(),
                    new StoreIndirectImporter(),
                    new LoadIndirectImporter(),
                    new StoreFieldImporter(),
                    new LoadFieldImporter(),
                    new BinaryOperationImporter(),
                    new CompareImporter(),
                    new BranchImporter(),
                    new LoadArgImporter(),
                    new StoreArgImporter(),
                    new LoadStringImporter(),
                    new InitobjImporter(),
                    new ConversionImporter(),
                    new NegImporter(),
                    new RetImporter(),
                    new CallImporter(),
                    new DupImporter(),
                    new NewobjImporter(),
                    new PopImporter(),
                    new AddressOfFieldImporter(),
                };
            }
            return _opcodeImporters;
        }
    }
}
