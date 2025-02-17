using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.Compiler.Importer
{
    public class NewarrImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.newarr) return false;

            var numElements = importer.PopExpression();

            var runtimeDeterminedType = (TypeDesc)instruction.Operand;
            var runtimeDeterminedArrayType = runtimeDeterminedType.MakeArrayType();

            StackEntry eeTypeNode;
            if (runtimeDeterminedType.IsRuntimeDeterminedSubtype)
            {
                // Only handle AcquiresInstMethodTableFromThis which will get
                // the EETypePtr from this pointer.
                Debug.Assert(context.Method.AcquiresInstMethodTableFromThis());

                eeTypeNode = context.GetGenericContext();
            }
            else
            {
                var mangledEETypeName = context.NameMangler.GetMangledTypeName(runtimeDeterminedArrayType);
                eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);
            }

            var args = new List<StackEntry>() { numElements, eeTypeNode };
            var node = new CallEntry("NewArray", args, VarType.Ref, 2);
            importer.PushExpression(node);

            return true;
        }
    }
}