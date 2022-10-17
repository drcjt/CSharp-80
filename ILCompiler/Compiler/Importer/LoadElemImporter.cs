using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Ldelem_I 
                || code == Code.Ldelem_I1 
                || code == Code.Ldelem_I2 
                || code == Code.Ldelem_I4
                || code == Code.Ldelem_U1
                || code == Code.Ldelem_U2
                || code == Code.Ldelem_U4;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();
            var op2 = importer.PopExpression();

            var elemType = GetType(instruction.OpCode.Code);
            var exactSize = elemType.GetTypeSize();

            var node = new IndexRefEntry(op1, op2, exactSize, elemType);

            importer.PushExpression(node);
        }

        private static VarType GetType(Code code)
        {
            return code switch
            {
                Code.Ldelem_I1 => VarType.SByte,
                Code.Ldelem_I2 => VarType.Short,
                Code.Ldelem_I4 => VarType.Int,
                Code.Ldelem_I => VarType.Ptr,
                Code.Ldelem_U1 => VarType.Byte,
                Code.Ldelem_U2 => VarType.UShort,
                Code.Ldelem_U4 => VarType.UInt,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
