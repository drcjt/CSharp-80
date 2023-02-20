using dnlib.DotNet;
using dnlib.DotNet.Emit;
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
            typeSig = context.Method.ResolveType(typeSig);
            VarType elemType = typeSig.GetVarType();
            int elemSize = typeSig.GetInstanceFieldSize();

            importer.PushExpression(new Int32ConstantEntry(checked((int)elemSize)));

            return true;
        }
    }
}
