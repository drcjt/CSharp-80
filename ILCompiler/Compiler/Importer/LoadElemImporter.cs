﻿using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Ldelem_I4;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();
            var op2 = importer.PopExpression();

            var node = new IndexRefEntry(op1, op2, 4, VarType.Int);

            importer.PushExpression(node);
        }
    }
}
