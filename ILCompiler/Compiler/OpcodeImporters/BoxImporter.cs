using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class BoxImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.box) return false;

            var objType = (TypeDesc)instruction.Operand;

            if (objType.IsValueType)
            {
                var value = importer.Pop();
                var boxedNode = BoxValue(value, objType, importer);
                importer.Push(boxedNode);
            }

            return true;
        }

        public static StackEntry BoxValue(StackEntry value, TypeDesc objType, IImporter importer)
        {
            string mangledEETypeName = importer.NameMangler.GetMangledTypeName(objType);
            var eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);

            // Spill value into a local so we can take its address to pass to the runtime helper
            // TODO: Optimize this to avoid the local if possible
            var lclNum = importer.GrabTemp(value.Type, value.ExactSize);
            var asg = new StoreLocalVariableEntry(lclNum, false, value);
            importer.ImportAppendTree(asg);

            var arguments = new List<StackEntry> { eeTypeNode, new LocalVariableAddressEntry(lclNum) };
            MethodDesc runtimeHelperMethod = importer.Method.Context.GetCoreLibEntryPoint("System.Runtime", "RuntimeExports", "Box");
            string mangledHelperMethod = importer.NameMangler.GetMangledMethodName(runtimeHelperMethod);
            var node = new CallEntry(mangledHelperMethod, arguments, VarType.Ref, null);

            return node;
        }
    }
}
