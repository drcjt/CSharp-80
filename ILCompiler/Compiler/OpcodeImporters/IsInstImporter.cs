﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class IsInstImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.isinst) return false;

            // Reference type to test
            var op1 = importer.Pop();

            // Determine type to check reference type against
            var typeDesc = (TypeDesc)instruction.Operand;

            string helperMethodName = "IsInstanceOfClass";
            if (typeDesc.IsArray)
            {
                throw new NotImplementedException();
            }
            else if (typeDesc.IsInterface)
            {
                helperMethodName = "IsInstanceOfInterface";
            }

            // Create call to helper method passing eetypeptr and object reference
            var lookup = importer.NodeFactory.NecessaryTypeSymbol(typeDesc);

            var args = new List<StackEntry>() { new NativeIntConstantEntry(lookup.MangledTypeName), op1 };
            var node = new CallEntry(GetHelperMethod(helperMethodName, importer), args, VarType.Ref, 2);

            importer.Push(node);

            return true;
        }

        private static string GetHelperMethod(string helperMethodName, IImporter importer)
        {
            var runtimeHelperMethod = importer.Method.Context.GetHelperEntryPoint("System.Runtime", "TypeCast", helperMethodName);
            var mangledHelperMethod = importer.NameMangler.GetMangledMethodName(runtimeHelperMethod);

            return mangledHelperMethod;
        }
    }
}