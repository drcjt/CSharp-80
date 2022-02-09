using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

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

            if (!declType.IsValueType)
            {
                throw new NotSupportedException("Newobj not supported for non value types");
            }

            // TODO: Put code here to allocate memory e.g. create an equivalent to a ryujit GT_CALL node

            var objType = declType.ToTypeSig();
            var objKind = objType.GetStackValueKind();

            var lclNum = importer.GrabTemp(objKind, objType.GetExactSize());
            var newObjThisPtr = new LocalVariableAddressEntry(lclNum);

            CallImporter.ImportCall(instruction, context, importer, newObjThisPtr);

            var node = new LocalVariableEntry(lclNum, objKind, objType.GetExactSize());
            importer.PushExpression(node);
        }
    }
}
