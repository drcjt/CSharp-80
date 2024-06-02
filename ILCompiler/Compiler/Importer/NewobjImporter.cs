using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NewobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Newobj) return false;

            var ctor = (IMemberRef)instruction.Operand;
            var owningType = context.Module.Create(ctor.DeclaringType);

            var method = context.Module.Create((IMethodDefOrRef)ctor);

            if (owningType.IsArray)
            {
                ImportNewObjArray(importer, (ArrayType)owningType);
            }
            else
            {
                if (IsSystemStringConstructor(method))
                {
                    ImportNewObjString(instruction, context, importer, method);
                }
                else
                {
                    var objType = (DefType)owningType;
                    var objVarType = objType.VarType;
                    var objSize = objType.InstanceFieldSize.AsInt;

                    if (objType.IsValueType)
                    {
                        ImportNewObjValueType(instruction, context, importer, objVarType, objSize);
                    }
                    else
                    {
                        ImportNewObjReferenceType(instruction, context, importer, objType, objVarType, objSize);
                    }
                }
            }

            return true;
        }

        private static bool IsSystemStringConstructor(MethodDesc methodToCall)
        {
            return methodToCall.OwningType.FullName == "System.String" &&
                   methodToCall.IsInternalCall &&
                   methodToCall.HasCustomAttribute("System.Diagnostics.CodeAnalysis", "DynamicDependencyAttribute");
        }

        private static void ImportNewObjReferenceType(Instruction instruction, ImportContext context, IILImporterProxy importer, DefType objType, VarType objVarType, int objSize)
        {
            var mangledEETypeName = context.NameMangler.GetMangledTypeName(objType);

            // Determine required size on GC heap
            var allocSize = objType.InstanceByteCount.AsInt;

            // Allocate memory for object
            var op1 = new AllocObjEntry(mangledEETypeName, allocSize, objVarType);

            // Store allocated memory address into a temp local variable
            var lclNum = importer.GrabTemp(VarType.Ref, objSize);
            var asg = new StoreLocalVariableEntry(lclNum, false, op1);
            importer.ImportAppendTree(asg);

            // Call the constructor                    
            var newObjThisPtr = new LocalVariableEntry(lclNum, objVarType, objSize);
            CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

            // Push a local variable entry corresponding to the object here
            var node = new LocalVariableEntry(lclNum, VarType.Ref, objSize);
            importer.PushExpression(node);
        }

        private static void ImportNewObjValueType(Instruction instruction, ImportContext context, IILImporterProxy importer, VarType objVarType, int objSize)
        {
            // Allocate memory on the stack for the value type as a temp local variable
            var lclNum = importer.GrabTemp(objVarType, objSize);
            var newObjThisPtr = new LocalVariableAddressEntry(lclNum);

            // Call the valuetype constructor
            CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

            var node = new LocalVariableEntry(lclNum, objVarType, objSize);
            importer.PushExpression(node);
        }

        private static void ImportNewObjString(Instruction instruction, ImportContext context, IILImporterProxy importer, MethodDesc methodToCall)
        {
            // String constructors marked as dynamic dependencies simply
            // call the referred method which will deal with allocation and
            // construction.
            var dependentTypeAttribute = methodToCall.CustomAttributes.Find("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");

            var dependentMethodName = dependentTypeAttribute.ConstructorArguments[0].Value.ToString();
            if (dependentMethodName == null) throw new InvalidOperationException("DynamicDependencyAttribute missing method name");

            var dependentMethod = methodToCall.OwningType.FindMethodEndsWith(dependentMethodName);
            if (dependentMethod == null) throw new InvalidOperationException($"Cannot find dynamic dependency {dependentMethodName}");

            // Replace the method to call in the instruction with the one referred to by the dynamic dependency attribute
            instruction.Operand = dependentMethod;

            // Call the dynamic dependency method
            CallImporter.ImportCall(dependentMethod, instruction, context, importer, null);
        }

        private static void ImportNewObjArray(IILImporterProxy importer, ArrayType arrayType)
        {
            // Extract element type and number of dimensions
            var rank = arrayType.Rank;
            var elemType = arrayType.ElementType;
            var elemSize = elemType.GetElementSize().AsInt;

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
            var node = new CallEntry("NewArray", args, VarType.Ref, 2);
            importer.PushExpression(node);
        }
    }
}
