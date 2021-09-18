using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class RetImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ret;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var retNode = new ReturnEntry();
            if (context.Method.HasReturnType)
            {
                var value = importer.PopExpression();

                if (value.Kind == StackValueKind.ValueType)
                {
                    // returning a struct
                    // so generate assignment to hidden return buffer argument first

                }

                if (value.Kind != StackValueKind.Int32)
                {
                    throw new NotSupportedException("Return values of types other than short and int32 not supported yet");
                }
                retNode.Return = value;
            }
            importer.ImportAppendTree(retNode);
            context.StopImporting = true;
        }
    }
}