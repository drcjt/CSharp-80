﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NewarrImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Newarr) return false;

            var op2 = importer.PopExpression();

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            typeSig = context.Method.ResolveType(typeSig);
            var arrayElementSize = typeSig.GetInstanceFieldSize();

            var elemTypeDef = (instruction.Operand as ITypeDefOrRef).ResolveTypeDefThrow();
            var mangledEETypeName = context.NameMangler.GetMangledTypeName(elemTypeDef);
            var eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);

            // Instead of creating new node type specifically for arrays
            // could leverage existing CallEntry node to call arbitrary helper functions
            // can use this then for other helper functions too

            var args = new List<StackEntry>() { op2, new Int32ConstantEntry(arrayElementSize), eeTypeNode };
            var node = new CallEntry("NewArray", args, VarType.Ref, 2);
            importer.PushExpression(node);

            return true;
        }
    }
}
