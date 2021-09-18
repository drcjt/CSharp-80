using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class NewobjImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Newobj;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;
            var methodToCall = methodDefOrRef.ResolveMethodDefThrow();

            var objType = methodToCall.DeclaringType;

            if (!objType.IsValueType)
            {
                throw new NotSupportedException("Newobj not supported for non value types");
            }

            // TODO: Grab Temp, Generate new GT_ADDR node
        }
    }
}
