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

                if (importer.Inlining)
                {
                    var inlineCandidateInfo = importer.InlineInfo!.InlineCandidateInfo;
                    var returnExpressionEntry = inlineCandidateInfo!.ReturnExpressionEntry;

                    if (returnBufferArgIndex.HasValue)
                    {
                        var returnBufferArg = importer.InlineInfo!.InlineCall.Arguments[returnBufferArgIndex.Value].Duplicate();
                        returnExpressionEntry!.SubstitionExpression = StoreStructPtr(returnBufferArg, returnValue, importer);
                    }
                    else
                    {
                        returnExpressionEntry!.SubstitionExpression = returnValue;
                    }
                }
            }

            if (importer.Inlining)
            {
                return true;
            }

            if (returnBufferArgIndex is not null)
            {
                var destination = new LocalVariableEntry(returnBufferArgIndex.Value, VarType.Ref, 2);
                var storeInd = StoreStructPtr(destination, returnValue!, importer);
                importer.ImportAppendTree(storeInd);

                returnValue = null;
            }
                
            var retNode = new ReturnEntry(returnValue);
            importer.ImportAppendTree(retNode);
            importer.StopImporting = true;

            return true;
        }

        private static StackEntry StoreStructPtr(StackEntry destinationAddress, StackEntry value, IImporter importer)
        {
            StackEntry? store;
            if (destinationAddress is LocalVariableAddressEntry localVariableAddress)
            {
                store = new StoreLocalVariableEntry(localVariableAddress.LocalNumber, true, value, VarType.Struct, value.ExactSize);
            }
            else
            {
                store = new StoreIndEntry(destinationAddress, value, value.Type, 0, value.ExactSize);
            }

            return importer.StoreStruct(store);
        }
    }
}