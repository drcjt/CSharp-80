using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class RetImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public RetImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ret;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var hasReturnValue = context.Method.HasReturnType;
            var retNode = new ReturnEntry();
            if (hasReturnValue)
            {
                var value = _importer.PopExpression();
                if (value.Kind != StackValueKind.Int32)
                {
                    throw new NotSupportedException("Return values of types other than short and int32 not supported yet");
                }
                retNode.Return = value;
            }
            _importer.ImportAppendTree(retNode);
            context.StopImporting = true;
        }
    }
}