using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NewobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Newobj) return false;

            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;

            if (methodDefOrRef == null)
            {
                throw new InvalidOperationException("Newobj importer called with Operand which isn't a IMethodDefOrRef");
            }

            var declaringTypeSig = methodDefOrRef.DeclaringType.ToTypeSig();

            if (declaringTypeSig.IsArray)
            {
                // Extract element type and number of dimensions
                var arraySig = declaringTypeSig.ToArraySig();
                var rank = arraySig.Rank;
                var elemType = arraySig.Next;   // TODO: Is this the right way to determine the element type?
                var elemSize = elemType.GetInstanceFieldSize();

                // Need to call helper to create MD array
                // The helper should take as arguments:
                //   * Number of dimensions
                //   * Elem Size
                //   * Size of each dimension
                // If we arrange the arguments such that the last item on the stack is the number of dimensions
                // then the helper can easily pop off each dimensions size.

                // Helper should calculate required size for the array
                // e.g. size of dimensions multipled together * elem size + (number of dimensions * 2)
                // This assumes dimension size can be no bigger than a short

                // Calculate size of dimensions e.g. dim1 * dim2 * dim3 * ...
                // TODO: currently does not allocate space for array bounds at all
                StackEntry sizeOp = importer.PopExpression();
                for (var dimension = 1; dimension < rank; dimension++)
                {
                    var dimensionOp = importer.PopExpression();
                    sizeOp = new BinaryOperator(Operation.Mul, isComparison: false, sizeOp, dimensionOp, VarType.Ptr);
                }

                // Create node to new up the array
                var args = new List<StackEntry>() { sizeOp, new Int32ConstantEntry(elemSize) };
                var node = new CallEntry("NewArr", args, VarType.Ref, 2);
                importer.PushExpression(node);
            }
            else
            {
                var methodToCall = methodDefOrRef.ResolveMethodDefThrow();
                var declType = methodToCall.DeclaringType;

                var objType = declType.ToTypeSig();
                var objVarType = objType.GetVarType();
                var objSize = objType.GetInstanceFieldSize();

                if (declType.IsValueType)
                {
                    // Allocate memory on the stack for the value type as a temp local variable
                    var lclNum = importer.GrabTemp(objVarType, objSize);
                    var newObjThisPtr = new LocalVariableAddressEntry(lclNum);

                    // Call the valuetype constructor
                    CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

                    var node = new LocalVariableEntry(lclNum, objVarType, objSize);
                    node.Type = VarType.Struct;
                    importer.PushExpression(node);
                }
                else
                {
                    // Determine required size on GC heap
                    var allocSize = objType.GetInstanceByteCount();

                    // Allocate memory for object
                    var op1 = new AllocObjEntry(allocSize, objVarType);

                    // Store allocated memory address into a temp local variable
                    var lclNum = importer.GrabTemp(VarType.Ptr, objSize);
                    var asg = new StoreLocalVariableEntry(lclNum, false, op1);
                    importer.ImportAppendTree(asg);

                    // Call the constructor                    
                    var newObjThisPtr = new LocalVariableEntry(lclNum, objVarType, objSize);
                    CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

                    // Push a local variable entry corresponding to the object here
                    var node = new LocalVariableEntry(lclNum, objVarType, objSize);
                    node.Type = VarType.Ptr;
                    importer.PushExpression(node);
                }
            }

            return true;
        }
    }
}
