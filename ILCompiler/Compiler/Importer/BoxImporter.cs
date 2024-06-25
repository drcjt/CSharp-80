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
                var lclNum = importer.GrabTemp(VarType.Ref, 2);

                var mangledEETypeName = context.NameMangler.GetMangledTypeName(objType);

                // Determine required size on GC heap
                var allocSize = ((DefType)objType).InstanceByteCount.AsInt;

                // Allocate memory for object
                var op1 = new AllocObjEntry(mangledEETypeName, allocSize, VarType.Ref);
                var asg = new StoreLocalVariableEntry(lclNum, false, op1);
                importer.ImportAppendTree(asg);

                // Copy value type into box
                // TODO:change this to use cpobj when implemented
                var newObjThisPtr = new LocalVariableEntry(lclNum, VarType.Ref, 2);
                var headerOffset = new NativeIntConstantEntry(2);
                var addr = new BinaryOperator(Operation.Add, isComparison: false, newObjThisPtr, headerOffset, VarType.Ptr);

                var value = importer.PopExpression();

                var op = new StoreIndEntry(addr, value, VarType.Ref);
                importer.ImportAppendTree(op);

                // Push temp
                var node = new LocalVariableEntry(lclNum, VarType.Ref, 2);
                importer.PushExpression(node);
            }

            return true;
        }
    }
}
