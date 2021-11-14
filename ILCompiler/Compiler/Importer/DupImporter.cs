using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class DupImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Dup;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();
            var op2 = op1.Duplicate();
            importer.PushExpression(op1);
            importer.PushExpression(op2);
        }
    }
}
