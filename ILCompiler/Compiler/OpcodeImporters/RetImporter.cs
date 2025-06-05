using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class RetImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ret) return false;

            StackEntry? returnValue = null;
            int? returnBufferArgIndex = null;
            int? returnTypeExactSize = null;

            if (importer.Method.HasReturnType)
            {
                var returnType = importer.Method.Signature.ReturnType;
                var value = importer.Pop();

                if (returnType.IsValueType && !returnType.IsPrimitive && !returnType.IsEnum) // && returnType.GetElementSize().AsInt > 4)
                {
                    // Record return buffer argument index
                    // so that code gen can generate code to
                    // copy struct on top of stack to the 
                    // return buffer.
                    returnBufferArgIndex = importer.Method.HasThis ? 1 : 0;
                    returnTypeExactSize = returnType.GetElementSize().AsInt;
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

            if (importer.Inlining)
                return true;

            var retNode = new ReturnEntry(returnValue, returnBufferArgIndex, returnTypeExactSize);
            importer.ImportAppendTree(retNode);
            importer.StopImporting = true;

            return true;
        }
    }
}