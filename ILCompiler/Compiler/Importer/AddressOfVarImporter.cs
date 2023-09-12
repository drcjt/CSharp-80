using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldloca:
                case Code.Ldloca_S:
                    var localNumber = importer.ParameterCount + (instruction.OperandAs<Local>()).Index;
                    importer.PushExpression(new LocalVariableAddressEntry(localNumber));
                    importer.LocalVariableTable[localNumber].AddressExposed = true;
                    return true;
                default:
                    return false;
            }
        }
    }
}
