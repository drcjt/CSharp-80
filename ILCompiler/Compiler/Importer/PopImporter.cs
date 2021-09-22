using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class PopImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Pop;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            importer.PopExpression();
        }
    }
}
