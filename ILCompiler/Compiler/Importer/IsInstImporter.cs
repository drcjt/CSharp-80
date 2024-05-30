using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class IsInstImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Isinst) return false;

            // Reference type to test
            var op1 = importer.PopExpression();

            // Determine type to check reference type against
            var typeDesc = context.TypeSystemContext.Create((ITypeDefOrRef)instruction.Operand);

            string helperMethodName = "IsInstanceOfClass";
            if (typeDesc.IsArray)
            {
                throw new NotImplementedException();
            }
            else if (typeDesc.IsInterface)
            {
                throw new NotImplementedException("IsInst only supports class types");
            }

            // Create call to helper method passing eetypeptr and object reference
            var lookup = context.NodeFactory.NecessaryTypeSymbol(typeDesc);

            var args = new List<StackEntry>() { new NativeIntConstantEntry(lookup.MangledTypeName), op1 };
            var node = new CallEntry(GetHelperMethod(context, helperMethodName), args, VarType.Ref, 2);

            importer.PushExpression(node);

            return true;
        }

        private static string GetHelperMethod(ImportContext context, string helperMethodName)
        {
            var systemRuntimeTypeCast = context.CorLibModuleProvider.FindThrow("System.Runtime.TypeCast");
            var runtimeHelperMethod = systemRuntimeTypeCast.FindMethod(helperMethodName);
            var mangledHelperMethod = context.NameMangler.GetMangledMethodName(runtimeHelperMethod);

            return mangledHelperMethod;
        }
    }
}
