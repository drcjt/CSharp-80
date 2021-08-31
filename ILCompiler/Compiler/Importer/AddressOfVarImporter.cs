using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfVarImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public AddressOfVarImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldloca || opcode == Code.Ldloca_S;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var index = (instruction.Operand as Local).Index;
            var localNumber = _importer.ParameterCount + index;
            var node = new LocalVariableAddressEntry(localNumber);
            _importer.PushExpression(node);
        }
    }
}
