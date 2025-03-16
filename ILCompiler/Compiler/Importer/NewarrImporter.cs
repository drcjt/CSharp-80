using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
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

            var runtimeHelperMethod = context.Method.Context.GetHelperEntryPoint("System.Runtime", "RuntimeImports", "NewArray");

            var args = new List<StackEntry>() { eeTypeNode, numElements };
            var node = new CallEntry("NewArray", args, VarType.Ref, 2, runtimeHelperMethod.IsVirtual, runtimeHelperMethod);
            importer.PushExpression(node);

            return true;
        }
    }
}