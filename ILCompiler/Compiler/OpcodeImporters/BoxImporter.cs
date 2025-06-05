using ILCompiler.Compiler.EvaluationStack;
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
            // TODO: Consider creating a bespoke Box assembly code runtime routine to do the below
            // Note - nativeaot implements this in C# in System.Runtime.RuntimeExports.RhBox

            var lclNum = importer.GrabTemp(VarType.Ref, 2);

            var mangledEETypeName = importer.NameMangler.GetMangledTypeName(objType);
            var eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);

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
