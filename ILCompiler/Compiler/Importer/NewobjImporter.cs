using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NewobjImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Newobj;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;

            if (methodDefOrRef == null)
            {
                throw new InvalidOperationException("Newobj importer called with Operand which isn't a IMethodDefOrRef");
            }

            var declaringTypeSig = methodDefOrRef.DeclaringType.ToTypeSig();

            if (declaringTypeSig.IsArray)
            {
                // TODO: Multidimensional arrays
                // Need to extract element type and number of dimensions

                var arraySig = declaringTypeSig.ToArraySig();
                var rank = arraySig.Rank;
                var elemType = arraySig.Next;   // TODO: Is this the right way to determine the element type?

                throw new NotImplementedException("Multidimensional arrays not supported");
            }
            else
            {
                var methodToCall = methodDefOrRef.ResolveMethodDefThrow();
                var declType = methodToCall.DeclaringType;

                var objType = declType.ToTypeSig();
                var objVarType = objType.GetVarType();
                var objSize = objType.GetExactSize();

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
                    // Allocate memory for object
                    var op1 = new AllocObjEntry((int)declType.ClassSize, objVarType);

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
        }
    }
}
