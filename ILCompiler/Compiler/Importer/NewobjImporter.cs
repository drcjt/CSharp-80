using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
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
            var methodToCall = methodDefOrRef.ResolveMethodDefThrow();

            var declType = methodToCall.DeclaringType;

            var objType = declType.ToTypeSig();
            var objKind = objType.GetStackValueKind();
            var objSize = objType.GetExactSize();

            if (declType.IsValueType)
            {
                // Allocate memory on the stack for the value type as a temp local variable
                var lclNum = importer.GrabTemp(objKind, objSize);
                var newObjThisPtr = new LocalVariableAddressEntry(lclNum);

                // Call the valuetype constructor
                CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

                var node = new LocalVariableEntry(lclNum, objKind, objSize);
                importer.PushExpression(node);
            }
            else
            {
                // Allocate memory for object
                var op1 = new AllocObjEntry((int)declType.ClassSize, objKind);

                // Store allocated memory address into a temp local variable
                var lclNum = importer.GrabTemp(objKind, objSize);
                var asg = new StoreLocalVariableEntry(lclNum, false, op1);
                importer.ImportAppendTree(asg);

                // Call the constructor
                var newObjThisPtr = new LocalVariableEntry(lclNum, objKind, objSize);
                CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

                // Push a local variable entry corresponding to the object here
                var node = new LocalVariableEntry(lclNum, objKind, objSize);
                importer.PushExpression(node);
            }
        }
    }
}
