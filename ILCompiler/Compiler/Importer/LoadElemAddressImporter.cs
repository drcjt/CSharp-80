using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Helpers;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Ldelema) return false;

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            var typeDesc = context.TypeSystemContext.Create(typeSig, context.Method.Instantiation);
            var elemType = typeDesc.VarType;
            int elemSize = typeDesc.GetElementSize().AsInt;

            // TODO: Consider using a helper function instead
            // ryujit uses a helper function here CORINFO_HELP_LDELEMA_REF

            var index = importer.PopExpression();
            var arrayRef = importer.PopExpression();

            var checkBounds = !context.Configuration.SkipArrayBoundsCheck;

            var arrayElementHelper = new ArrayElementHelper(importer.LocalVariableTable);

            StackEntry addr = arrayElementHelper.CreateArrayAccess(index, arrayRef, elemType, elemSize, true, checkBounds, 2);

            importer.PushExpression(addr);

            return true;
        }
    }
}
