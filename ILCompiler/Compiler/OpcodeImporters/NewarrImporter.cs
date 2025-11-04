using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class NewarrImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.newarr) return false;

            var numElements = importer.Pop();

            var runtimeDeterminedType = (TypeDesc)instruction.Operand;
            var runtimeDeterminedArrayType = runtimeDeterminedType.MakeArrayType();

            StackEntry eeTypeNode;
            if (runtimeDeterminedType.IsRuntimeDeterminedSubtype)
            {
                // Only handle AcquiresInstMethodTableFromThis which will get
                // the EETypePtr from this pointer.
                Debug.Assert(importer.Method.AcquiresInstMethodTableFromThis());

                eeTypeNode = importer.GetGenericContext();
            }
            else
            {
                var mangledEETypeName = importer.NameMangler.GetMangledTypeName(runtimeDeterminedArrayType);
                eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);
            }

            var runtimeHelperMethod = importer.Method.Context.GetCoreLibEntryPoint("System.Runtime", "RuntimeImports", "NewArray");

            var args = new List<StackEntry>() { eeTypeNode, numElements };
            var node = new CallEntry("NewArray", args, VarType.Ref, 2, runtimeHelperMethod.IsVirtual, runtimeHelperMethod);
            importer.Push(node);

            return true;
        }
    }
}