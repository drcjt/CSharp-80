using dnlib.DotNet;
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
                || code == Code.Ldelem_U4
                || code == Code.Ldelem_Ref
                || code == Code.Ldelem;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();
            var op2 = importer.PopExpression();

            int elemSize;
            VarType elemType;
            if (instruction.OpCode.Code == Code.Ldelem)
            {
                var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
                elemType = typeSig.GetVarType();
                elemSize = typeSig.GetExactSize();
            }
            else
            {
                elemType = GetType(instruction.OpCode.Code);
                elemSize = elemType.GetTypeSize();
            }

            var node = new IndexRefEntry(op1, op2, elemSize, elemType);

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
                Code.Ldelem_Ref => VarType.Ref,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
