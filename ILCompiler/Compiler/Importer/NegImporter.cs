using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NegImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public NegImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Neg;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var op1 = _importer.PopExpression();
            op1 = new UnaryOperator(Operation.Neg, op1);
            _importer.PushExpression(op1);
        }
    }
}
