﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfFieldImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldflda;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var fieldDef = instruction.Operand as FieldDef;

            var obj = importer.PopExpression();


            if (obj.Kind != StackValueKind.ObjRef && obj.Kind != StackValueKind.ByRef)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Kind}");
            }

            var node = new FieldAddressEntry(obj, fieldDef);

            importer.PushExpression(node);
        }
    }
}