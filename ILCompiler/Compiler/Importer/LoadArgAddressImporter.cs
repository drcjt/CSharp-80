using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadArgAddressImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ldarga || code == Code.Ldarga_S;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var index = (instruction.OperandAs<Parameter>()).Index;
            var lclNum = MapIlArgNum(index, importer.ReturnBufferArgIndex);

            importer.PushExpression(new LocalVariableAddressEntry(lclNum));
        }

        /// <summary>
        /// Map IL arg num to account for hidden parameters
        /// </summary>
        /// <param name="ilArgNum"></param>
        /// <returns></returns>
        private static int MapIlArgNum(int ilArgNum, int? returnBufferArgIndex)
        {
            if (returnBufferArgIndex.HasValue)
            {
                if (ilArgNum >= returnBufferArgIndex)
                {
                    ilArgNum++;
                }
            }

            return ilArgNum;
        }
    }
}
