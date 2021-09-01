using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadStringImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ldstr;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            importer.PushExpression(new StringConstantEntry(instruction.Operand as string));
        }
    }
}
