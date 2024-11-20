using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class BoxImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.box) return false;

            var objType = (TypeDesc)instruction.GetOperand();

            if (objType.IsValueType)
            {
                // TODO: Consider creating a bespoke Box assembly code runtime routine to do the below
                // Note - nativeaot implements this in C# in System.Runtime.RuntimeExports.RhBox

                var lclNum = importer.GrabTemp(VarType.Ref, 2);

                var mangledEETypeName = context.NameMangler.GetMangledTypeName(objType);
                var eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);

                int unboxedObjectSize = GetUnboxedSize(objType);

                // Determine required size on GC heap. Have to explicitly add PointerSize for EETypePtr
                var allocSize = unboxedObjectSize + 2;

                // Allocate memory for object
                var op1 = new AllocObjEntry(eeTypeNode, allocSize, VarType.Ref);
                var asg = new StoreLocalVariableEntry(lclNum, false, op1);
                importer.ImportAppendTree(asg);

                // Copy value type into box
                var newObjThisPtr = new LocalVariableEntry(lclNum, VarType.Ref, 2);
                var headerOffset = new NativeIntConstantEntry(2);
                var addr = new BinaryOperator(Operation.Add, isComparison: false, newObjThisPtr, headerOffset, VarType.Ptr);

                var value = importer.PopExpression();

                var op = new StoreIndEntry(addr, value, VarType.Ref, 0, unboxedObjectSize);
                importer.ImportAppendTree(op);

                // Push temp
                var node = new LocalVariableEntry(lclNum, VarType.Ref, 2);
                importer.PushExpression(node);
            }

            return true;
        }

        private static int GetUnboxedSize(TypeDesc objType)
        {
            // On stack we either have
            // * structs - which take up exact space, so struct with a byte in it is 1 byte long
            // * native signed/unsigned ints - which take up 2 bytes
            // * everything else takes up 4 bytes

            var unboxedObjectSize = 4;
            if (objType.VarType == VarType.Struct)
            {
                unboxedObjectSize = objType.GetElementSize().AsInt;
            }
            if (objType.VarType.GenActualTypeIsI())
            {
                unboxedObjectSize = 2;
            }

            return unboxedObjectSize;
        }
    }
}
