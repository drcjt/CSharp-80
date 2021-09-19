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

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;
            var methodToCall = methodDefOrRef.ResolveMethodDefThrow();

            var objType = methodToCall.DeclaringType;

            if (!objType.IsValueType)
            {
                throw new NotSupportedException("Newobj not supported for non value types");
            }

            var lclNum = importer.GrabTemp();

            //TODO: Should this be in GrabTemp?
            var temp = importer.LocalVariableTable[lclNum];
            temp.Type = objType.ToTypeSig();
            temp.Kind = objType.ToTypeSig().GetStackValueKind();

            var newObjThisPtr = new LocalVariableAddressEntry(lclNum);

            // Add newObjThisPtr as first parameter to call to constructor
            var arguments = new StackEntry[methodToCall.Parameters.Count];
            arguments[0] = newObjThisPtr;
            for (var i = 1; i < methodToCall.Parameters.Count; i++)
            {
                var argument = importer.PopExpression();
                arguments[methodToCall.Parameters.Count - i] = argument;
            }
            var targetMethod = context.NameMangler.GetMangledMethodName(methodToCall);
            var returnType = methodToCall.ReturnType.GetStackValueKind();

            var callNode = new CallEntry(targetMethod, arguments, returnType);

            // ctor has no return type so just append the tree
            importer.ImportAppendTree(callNode);

            var node = new LocalVariableEntry(lclNum, temp.Kind, temp.Type);
            importer.PushExpression(node);
        }
    }
}
