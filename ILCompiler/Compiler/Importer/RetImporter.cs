using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class RetImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode != OpCodes.Ret) return false;

            StackEntry? returnValue = null;
            int? returnBufferArgIndex = null;
            int? returnTypeExactSize = null;

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
                    returnBufferArgIndex = context.Method.HasThis ? 1 : 0;
                    returnTypeExactSize = returnType.GetInstanceFieldSize();
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

                returnValue = value;
            }

            var retNode = new ReturnEntry(returnValue, returnBufferArgIndex, returnTypeExactSize);
            importer.ImportAppendTree(retNode);
            context.StopImporting = true;

            return true;
        }
    }
}