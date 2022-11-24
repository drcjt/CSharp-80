using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class InitobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Initobj) return false;

            // TODO: Need to implement this
            importer.PopExpression();
            return true;
        }
    }
}
