using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class InitobjImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Initobj;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            // TODO: Need to implement this
            importer.PopExpression();
        }
    }
}
