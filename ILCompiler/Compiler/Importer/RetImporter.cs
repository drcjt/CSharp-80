﻿using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class RetImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Ret;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var retNode = new ReturnEntry();
            if (context.Method.HasReturnType)
            {
                var returnType = context.Method.ReturnType;

                var value = importer.PopExpression();

                if (returnType.IsStruct())
                {
                    // Record return buffer argument index
                    // so that code gen can generate code to
                    // copy struct on top of stack to the 
                    // return buffer.
                    retNode.ReturnBufferArgIndex = context.Method.HasThis ? 1 : 0;
                    retNode.ReturnTypeExactSize = returnType.GetExactSize();
                }
                else
                {
                    if (!value.Type.IsInt()
                        && value.Type != VarType.Ptr 
                        && value.Type != VarType.ByRef
                        && value.Type != VarType.Ref)
                    {
                        throw new NotSupportedException($"Unsupported Return type {value.Type}");
                    }
                }

                retNode.Return = value;
            }
            importer.ImportAppendTree(retNode);
            context.StopImporting = true;
        }
    }
}