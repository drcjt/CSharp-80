using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class UnboxImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.unbox_any && instruction.Opcode != ILOpcode.unbox) return false;

            var objType = (TypeDesc)instruction.GetOperand();

            if (!objType.IsValueType)
            {
                throw new NotImplementedException();
            }

            var boxedObject = importer.PopExpression();

            // Calculate address of the data in the box
            var headerOffset = new NativeIntConstantEntry(2);
            var addr = new BinaryOperator(Operation.Add, isComparison: false, boxedObject, headerOffset, VarType.Ptr);

            if (instruction.Opcode == ILOpcode.unbox)
            {
                // Push address of boxedObject
                importer.PushExpression(addr);
            }
            else
            {
                // Now copy the data to the stack
                var size = objType.GetElementSize().AsInt;

                var structVal = new IndirectEntry(addr, objType.VarType, size);
                importer.PushExpression(structVal);
            }

            return true;
        }
    }
}
