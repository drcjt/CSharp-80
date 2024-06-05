using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class SizeOfImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Sizeof) return false;

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            var typeDesc = context.Module.Create(typeSig, context.Method.Instantiation);

            var elemType = typeDesc.VarType;
            int elemSize = typeDesc.GetElementSize().AsInt;

            importer.PushExpression(new Int32ConstantEntry(checked((int)elemSize)));

            return true;
        }
    }
}
