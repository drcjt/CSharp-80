using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class NewobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
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
                    ImportNewObjString(instruction, importer, runtimeDeterminedMethod);
                }
                else
                {
                    var objType = (DefType)owningType;
                    var objVarType = objType.VarType;
                    var objSize = objType.InstanceFieldSize.AsInt;

                    if (objType.IsValueType)
                    {
                        ImportNewObjValueType(instruction, importer, objVarType, objSize);
                    }
                    else
                    {
                        ImportNewObjReferenceType(instruction, importer, objType, objVarType, objSize);
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

        private static void ImportNewObjReferenceType(Instruction instruction, IImporter importer, DefType objType, VarType objVarType, int objSize)
        {
            StackEntry eeTypeNode;
            if (objType.IsRuntimeDeterminedSubtype)
            {
                // Only handle AcquiresInstMethodTableFromThis which will get
                // the EETypePtr from this pointer.
                Debug.Assert(importer.Method.AcquiresInstMethodTableFromThis());

                eeTypeNode = importer.GetGenericContext();
            }
            else
            {
                var mangledEETypeName = importer.NameMangler.GetMangledTypeName(objType);
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
            CallImporter.ImportCall(instruction, importer, newObjThisPtr);

            // Push a local variable entry corresponding to the object here
            var node = new LocalVariableEntry(lclNum, VarType.Ref, objSize);
            importer.Push(node);
        }

        private static void ImportNewObjValueType(Instruction instruction, IImporter importer, VarType objVarType, int objSize)
        {
            // Allocate memory on the stack for the value type as a temp local variable
            var lclNum = importer.GrabTemp(objVarType, objSize);
            var newObjThisPtr = new LocalVariableAddressEntry(lclNum);

            // Call the valuetype constructor
            CallImporter.ImportCall(instruction, importer, newObjThisPtr);

            var node = new LocalVariableEntry(lclNum, objVarType, objSize);
            importer.Push(node);
        }

        private static void ImportNewObjString(Instruction instruction, IImporter importer, MethodDesc methodToCall)
        {
            // String constructors marked as dynamic dependencies simply
            // call the referred method which will deal with allocation and
            // construction.
            var dependentMethodName = methodToCall.GetCustomAttributeValue("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");
            if (dependentMethodName == null) throw new InvalidOperationException("DynamicDependencyAttribute missing method name");

            var dependentMethod = methodToCall.OwningType.FindMethodEndsWith(dependentMethodName);
            if (dependentMethod == null) throw new InvalidOperationException($"Cannot find dynamic dependency {dependentMethodName}");

            // Call the dynamic dependency method
            CallImporter.ImportCall(dependentMethod, instruction, importer, null);
        }

        /// <summary>
        /// All zero based one dimensional arrays, SZArrays, are created using Newarr.
        /// Only multidimensional arrays, or one-dimensional arrays which aren't zero based
        /// are created by Newobj
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="arrayType"></param>
        private static void ImportNewObjArray(IImporter importer, ArrayType arrayType)
        {
            Debug.Assert(arrayType.Rank > 0);

            var mangledEETypeName = importer.NameMangler.GetMangledTypeName(arrayType);
            var eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);
            var rank = new Int32ConstantEntry(arrayType.Rank);

            // Allocate a local temp to hold the array dimensions
            var dimensionsLocalNumber = importer.GrabTemp(VarType.Struct, arrayType.Rank * 4);

            var pLengths = new LocalVariableAddressEntry(dimensionsLocalNumber);
            StackEntry node = pLengths;

            // Initialize the dimensions
            for (int i = arrayType.Rank - 1; i >= 0; i--)
            {
                var dimension = importer.Pop();
                var store = new StoreIndEntry(new LocalVariableAddressEntry(dimensionsLocalNumber), dimension, VarType.Int, (uint)(i * 4));
                node = new CommaEntry(store, node);
            }

            var args = new List<StackEntry>() { eeTypeNode, rank, node };

            var runtimeHelperMethod = importer.Method.Context.GetHelperEntryPoint("Internal.Runtime.CompilerHelpers", "ArrayHelpers", "NewObjArray");
            var mangledHelperMethod = importer.NameMangler.GetMangledMethodName(runtimeHelperMethod);

            node = new CallEntry(mangledHelperMethod, args, VarType.Ref, 2);

            importer.Push(node);
        }
    }
}