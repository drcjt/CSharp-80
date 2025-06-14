using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class AddressOfVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.ldloca:
                case ILOpcode.ldloca_s:
                    var localVariableDefinition = (LocalVariableDefinition)instruction.Operand;

                    if (importer.Inlining)
                    {
                        var localNumber = importer.InlineFetchLocal(localVariableDefinition.Index);
                        importer.Push(new LocalVariableAddressEntry(localNumber));
                        importer.InlineInfo!.InlineLocalVariableTable[localNumber].AddressExposed = true;
                    }
                    else
                    {
                        var localNumber = localVariableDefinition.Index + importer.ParameterCount;
                        importer.Push(new LocalVariableAddressEntry(localNumber));
                        importer.LocalVariableTable[localNumber].AddressExposed = true;
                    }

                    return true;
                default:
                    return false;
            }
        }
    }
}
