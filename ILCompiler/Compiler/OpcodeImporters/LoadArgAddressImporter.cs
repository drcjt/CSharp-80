using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadArgAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.ldarga:
                case ILOpcode.ldarga_s:
                    var parameter = (ParameterDefinition)instruction.Operand;
                    var index = parameter.Index;

                    if (importer.Inlining)
                    {
                        LocalVariableEntry argument = (LocalVariableEntry)importer.InlineFetchArgument(index);
                        importer.Push(new LocalVariableAddressEntry(argument.LocalNumber));
                        importer.InlineInfo!.InlineLocalVariableTable[argument.LocalNumber].AddressExposed = true;
                    }
                    else
                    {
                        var localNumber = importer.MapIlArgNum(index);
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