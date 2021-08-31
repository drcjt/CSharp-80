using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadStringImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public LoadStringImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldstr;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var str = instruction.Operand as string;
            _importer.PushExpression(new StringConstantEntry(str));
        }
    }
}
