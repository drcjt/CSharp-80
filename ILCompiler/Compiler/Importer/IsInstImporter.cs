using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class IsInstImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.isinst) return false;

            // Reference type to test
            var op1 = importer.PopExpression();

            // Determine type to check reference type against
            var typeDesc = (TypeDesc)instruction.Operand;

            string helperMethodName = "IsInstanceOfClass";
            if (typeDesc.IsArray)
            {
                throw new NotImplementedException();
            }
            else if (typeDesc.IsInterface)
            {
                helperMethodName = "IsInstanceOfInterface";
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
            var runtimeHelperMethod = context.Method.Context.GetHelperEntryPoint("System.Runtime", "TypeCast", helperMethodName);
            var mangledHelperMethod = context.NameMangler.GetMangledMethodName(runtimeHelperMethod);

            return mangledHelperMethod;
        }
    }
}