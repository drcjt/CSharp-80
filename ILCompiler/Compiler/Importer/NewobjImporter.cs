using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.Compiler.Importer
{
    public class NewobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.newobj) return false;

            var runtimeDeterminedMethod = (MethodDesc)instruction.Operand;

            var owningType = runtimeDeterminedMethod.OwningType;

            if (owningType.IsArray)
            {
                ImportNewObjArray(importer, (ArrayType)owningType);
            }
            else
            {
                if (IsSystemStringConstructor(runtimeDeterminedMethod))
                {
                    ImportNewObjString(instruction, context, importer, runtimeDeterminedMethod);
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
            StackEntry eeTypeNode;
            if (objType.IsRuntimeDeterminedSubtype)
            {
                // Only handle AcquiresInstMethodTableFromThis which will get
                // the EETypePtr from this pointer.
                Debug.Assert(context.Method.AcquiresInstMethodTableFromThis());

                eeTypeNode = context.GetGenericContext();
            }
            else
            {
                var mangledEETypeName = context.NameMangler.GetMangledTypeName(objType);
                eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);
            }

            // Allocate memory for object
            var op1 = new AllocObjEntry(eeTypeNode, objVarType);

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
            var dependentMethodName = methodToCall.GetCustomAttributeValue("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");
            if (dependentMethodName == null) throw new InvalidOperationException("DynamicDependencyAttribute missing method name");

            var dependentMethod = methodToCall.OwningType.FindMethodEndsWith(dependentMethodName);
            if (dependentMethod == null) throw new InvalidOperationException($"Cannot find dynamic dependency {dependentMethodName}");

            // Call the dynamic dependency method
            CallImporter.ImportCall(dependentMethod, instruction, context, importer, null);
        }

        /// <summary>
        /// All zero based one dimensional arrays, SZArrays, are created using Newarr.
        /// Only multidimensional arrays, or one-dimensional arrays which aren't zero based
        /// are created by Newobj
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="arrayType"></param>
        private static void ImportNewObjArray(IILImporterProxy importer, ArrayType arrayType)
        {
            throw new NotImplementedException();
        }
    }
}