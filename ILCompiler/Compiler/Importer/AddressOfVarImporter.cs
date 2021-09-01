using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfVarImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldloca || opcode == Code.Ldloca_S;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            importer.PushExpression(new LocalVariableAddressEntry(importer.ParameterCount + (instruction.Operand as Local).Index));
        }
    }
}
