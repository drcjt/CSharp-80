using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LocallocImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Localloc;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();

            var allocSize = 0;
            if (op2 is Int32ConstantEntry)
            {
                allocSize = op2.As<Int32ConstantEntry>().Value;
            }
            else if (op2 is CastEntry)
            {
                var castOp = op2.As<CastEntry>();
                if (castOp.Op1 is Int32ConstantEntry)
                {
                    // TODO: Really need to use 16 bit version of this value
                    allocSize = castOp.Op1.As<Int32ConstantEntry>().Value;
                }
            }

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
