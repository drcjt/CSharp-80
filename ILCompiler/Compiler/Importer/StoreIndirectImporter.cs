﻿using dnlib.DotNet.Emit;
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

            if (addr.Type == VarType.Int)
            {
                var cast = new CastEntry(WellKnownType.IntPtr, addr, VarType.Ptr);
                cast.DesiredType2 = VarType.Ptr;
                addr = cast;
            }    

            WellKnownType type = GetWellKnownType(instruction);            
            int exactSize = type.GetWellKnownTypeSize();

            var node = new StoreIndEntry(addr, value, type, fieldOffset: 0, exactSize);
            node.Type = value.Type;

            importer.ImportAppendTree(node);
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
