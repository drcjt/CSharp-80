using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class InitobjImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Initobj;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            // TODO: Need to implement this
            importer.PopExpression();
        }
    }
}
