using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class LocallocImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Localloc;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var op2 = importer.PopExpression().As<Int32ConstantEntry>();

            if (op2.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Localloc requires int size");
            }

            var allocSize = op2.Value;

            // Ensure we don't allocate less than each stack entry size
            allocSize = RoundUp(allocSize, 4);

            // TODO: Is Unknown the right kind to use??
            var lclNum = importer.GrabTemp(StackValueKind.Unknown, allocSize);

            var op1 = new LocalVariableAddressEntry(lclNum);

            importer.PushExpression(op1);
        }

        /// <summary>
        /// Roundup size of data to allocate to specific multiple
        /// </summary>
        /// 
        // TODO: Consider moving to a LocalVariables class?
        private int RoundUp(int size, int multiplie)
        {
            return (size + (multiplie - 1)) & ~(multiplie - 1);
        }
    }
}
