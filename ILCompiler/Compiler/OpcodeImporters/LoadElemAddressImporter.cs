using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Helpers;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal class LoadElemAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ldelema) return false;

            var typeDesc = (TypeDesc)instruction.Operand;
            var elemType = typeDesc.VarType;
            int elemSize = typeDesc.GetElementSize().AsInt;

            // TODO: Consider using a helper function instead
            // ryujit uses a helper function here CORINFO_HELP_LDELEMA_REF

            var index = importer.Pop();
            var arrayRef = importer.Pop();

            var checkBounds = !context.Configuration.SkipArrayBoundsCheck;

            var arrayElementHelper = new ArrayElementHelper(importer.LocalVariableTable);

            StackEntry addr = arrayElementHelper.CreateArrayAccess(index, arrayRef, elemType, elemSize, true, checkBounds, 2);

            importer.Push(addr);

            return true;
        }
    }
}
