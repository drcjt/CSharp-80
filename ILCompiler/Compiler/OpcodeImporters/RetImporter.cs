using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class RetImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ret) return false;

            importer.StopImporting = true;

            if (!importer.Method.HasReturnType)
            {
                // Handle simple case of void return type
                ImportVoidReturn(importer);
            }
            else
            {
                // We have a non void return type, get the actual return type and value
                var returnType = importer.Method.Signature.ReturnType;
                var value = importer.Pop();

                if (returnType.VarType == VarType.Struct)
                {
                    ImportStructReturn(value, importer);
                }
                else
                {
                    ImportNonStructReturn(value, importer);
                }
            }

            return true;
        }

        private static void ImportVoidReturn(IImporter importer)
        {
            if (!importer.Inlining)
            {
                // Void return type, just import a return
                var retNode = new ReturnEntry(null);
                importer.ImportAppendTree(retNode);
            }
        }

        private static void ImportStructReturn(StackEntry value, IImporter importer)
        {
            // We must be using a return buffer so work out the argument index for the return buffer pointer
            var returnBufferArgIndex = importer.Method.HasThis ? 1 : 0;

            if (!importer.Inlining)
            {
                // Store the return value into the return buffer
                var destination = new LocalVariableEntry(returnBufferArgIndex, VarType.Ref, 2);
                var storeInd = StoreStructPtr(destination, value, importer);
                importer.ImportAppendTree(storeInd);

                // Dummy return as return buffer will hold real return value
                var retNode = new ReturnEntry(null);
                importer.ImportAppendTree(retNode);
            }
            else
            {
                // If inlining then get the return buffer argument and set the return
                // substitution expression to be the store to the return buffer argument
                var inlineCandidateInfo = importer.InlineInfo!.InlineCandidateInfo;
                var returnExpressionEntry = inlineCandidateInfo!.ReturnExpressionEntry;

                var returnBufferArg = importer.InlineInfo!.InlineCall.Arguments[returnBufferArgIndex].Duplicate();
                value = StoreStructPtr(returnBufferArg, value, importer);

                returnExpressionEntry!.SubstitutionExpression = value;
            }
        }

        private static void ImportNonStructReturn(StackEntry value, IImporter importer)
        {
            if (!value.Type.IsInt() && value.Type != VarType.Ptr &&
                 value.Type != VarType.ByRef && value.Type != VarType.Ref)
            {
                throw new NotSupportedException($"Unsupported Return type {value.Type}");
            }

            if (!importer.Inlining)
            {
                var retNode = new ReturnEntry(value);
                importer.ImportAppendTree(retNode);
            }
            else
            {
                var inlineCandidateInfo = importer.InlineInfo!.InlineCandidateInfo;
                var returnExpressionEntry = inlineCandidateInfo!.ReturnExpressionEntry;

                if (importer.InlineInfo.InlineeReturnSpillTempNumber.HasValue)
                {
                    var store = importer.NewTempStore(importer.InlineInfo.InlineeReturnSpillTempNumber.Value, value);
                    importer.ImportAppendTree(store, true);

                    value = new LocalVariableEntry(importer.InlineInfo.InlineeReturnSpillTempNumber.Value, value.Type, value.ExactSize);
                }

                returnExpressionEntry!.SubstitutionExpression = value;
            }
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
