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

            if (objType.IsNullable)
            {
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
            else
            {
                // For non nullable boxing create IR to handle the
                // boxing for the specific T rather than use helper
                var lclNum = importer.GrabTemp(VarType.Ref, 2);

                int unboxedObjectSize = objType.GetElementSize().AsInt;

                // Allocate memory for object
                var op1 = new AllocObjEntry(eeTypeNode, VarType.Ref);
                var asg = new StoreLocalVariableEntry(lclNum, false, op1);
                importer.ImportAppendTree(asg);

                // Copy value type into box
                var newObjThisPtr = new LocalVariableEntry(lclNum, VarType.Ref, 2);
                var headerOffset = new NativeIntConstantEntry(2);
                var addr = new BinaryOperator(Operation.Add, isComparison: false, newObjThisPtr, headerOffset, VarType.Ptr);

                var op = new StoreIndEntry(addr, value, objType.VarType, 0, unboxedObjectSize);
                importer.ImportAppendTree(op);

                // Push temp
                var node = new LocalVariableEntry(lclNum, VarType.Ref, 2);
                return node;
            }
        }
    }
}
