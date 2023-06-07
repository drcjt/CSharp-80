using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Ldelema) return false;

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            typeSig = context.Method.ResolveType(typeSig);
            VarType elemType = typeSig.GetVarType();
            int elemSize = typeSig.GetInstanceFieldSize();

            var op1 = importer.PopExpression();
            var op2 = importer.PopExpression();

            // TODO
            // Consider using a helper function instead
            // ryujit uses a helper function here CORINFO_HELP_LDELEMA_REF

            var cast = new CastEntry(op1, VarType.Ptr);
            StackEntry addr = cast;
            if (elemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)elemSize);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, addr, VarType.Ptr);
            }

            // addr + arraySizeOffset + (elemSize * index)
            var arraySizeOffset = new NativeIntConstantEntry(4);
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, arraySizeOffset, VarType.Ptr);
            addr = new BinaryOperator(Operation.Add, isComparison: false, op2, addr, VarType.Ptr);

            importer.PushExpression(addr);

            return true;
        }
    }
}
