using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System.Data;
using System.Xml.Linq;

namespace ILCompiler.Compiler.Importer
{
    public class NewarrImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Newarr;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();

            var typeDefOrRef = instruction.Operand as ITypeDefOrRef;
            var typeDef = typeDefOrRef.ResolveTypeDefThrow();

            var arrayElementType = typeDef.ToTypeSig();
            var arrayElementSize = arrayElementType.GetHeapValueSize();

            // Instead of creating new node type specifically for arrays
            // could leverage existing CallEntry node to call arbitrary helper functions
            // can use this then for other helper functions too

            var args = new List<StackEntry>() { new Int32ConstantEntry(arrayElementSize), op2  };
            var node = new CallEntry("NewArr", args, StackValueKind.ObjRef, 2);
            importer.PushExpression(node);
        }
    }
}
