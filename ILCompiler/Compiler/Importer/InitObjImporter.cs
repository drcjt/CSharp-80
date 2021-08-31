using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class InitObjImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public InitObjImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Initobj;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            // TODO: Need to implement this
            _importer.PopExpression();
        }
    }
}
