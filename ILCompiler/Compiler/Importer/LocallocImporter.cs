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
            if (op2.Kind == StackValueKind.Int32)
            {
                // Insert cast from int32 to nativeint as cannot localloc more
                // space than the processor can address :)
                op2 = new CastEntry(Common.TypeSystem.WellKnownType.Object, op2, StackValueKind.NativeInt);
            }

            if (op2.Kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException("Localloc requires native int size");
            }

            var op1 = new LocalHeapEntry(op2);

            importer.PushExpression(op1);
        }
    }
}
