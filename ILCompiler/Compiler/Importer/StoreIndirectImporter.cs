using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreIndirectImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stind_I1 || code == Code.Stind_I2 || code == Code.Stind_I4 || code == Code.Stind_I;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var value = importer.PopExpression();
            var addr = importer.PopExpression();

            if (addr.Kind != StackValueKind.NativeInt 
                && addr.Kind != StackValueKind.ByRef
                && addr.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException($"Store indirect unsupported address of kind {addr.Kind}");
            }

            if (value.Kind != StackValueKind.Int32 
                && value.Kind != StackValueKind.NativeInt
                && value.Kind != StackValueKind.ByRef
                && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException($"Cannot store indirect value of kind {value.Kind}");
            }

            WellKnownType type = GetWellKnownType(instruction);            
            int exactSize = type.GetWellKnownTypeSize();

            importer.ImportAppendTree(new StoreIndEntry(addr, value, type, fieldOffset: 0, exactSize));
        }

        private static WellKnownType GetWellKnownType(Instruction instruction)
        {
            var type = instruction.OpCode.Code switch
            {
                Code.Stind_I1 => WellKnownType.SByte,
                Code.Stind_I2 => WellKnownType.Int16,
                Code.Stind_I4 => WellKnownType.Int32,
                Code.Stind_I => WellKnownType.Int16,
                _ => throw new NotImplementedException(),
            };
            return type;
        }
    }
}
