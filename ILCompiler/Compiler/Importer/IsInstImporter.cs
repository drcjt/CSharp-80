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
            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            typeSig = context.Method.ResolveType(typeSig);
            var typeDef = (instruction.Operand as ITypeDefOrRef).ResolveTypeDefThrow();

            // TODO: Implement array & interface IsInst
            if (typeSig.IsArray || typeDef.IsInterface)
            {
                throw new NotImplementedException("IsInst only supports class types");
            }

            // Create call to helper method passing eetypeptr and object reference
            var mangledEETypeName = context.NameMangler.GetMangledTypeName(typeDef);
            var args = new List<StackEntry>() { new NativeIntConstantEntry(mangledEETypeName), op1 };
            var node = new CallEntry(GetHelperMethod(context), args, VarType.Ref, 2);

            importer.PushExpression(node);

            return true;
        }

        private static string GetHelperMethod(ImportContext context)
        {
            var systemRuntimeTypeCast = context.CorLibModuleProvider.FindThrow("System.Runtime.TypeCast");
            var runtimeHelperMethod = systemRuntimeTypeCast.FindMethod("IsInstanceOfClass");
            var mangledHelperMethod = context.NameMangler.GetMangledMethodName(runtimeHelperMethod);

            return mangledHelperMethod;
        }
    }
}
