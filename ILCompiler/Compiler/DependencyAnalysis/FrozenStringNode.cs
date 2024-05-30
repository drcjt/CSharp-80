using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class FrozenStringNode : DependencyNode
    {
        private readonly INameMangler _nameMangler;
        private readonly string _data;
        public readonly string Label;
        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly TypeSystemContext _typeSystemContext;

        public FrozenStringNode(string data, INameMangler nameMangler, CorLibModuleProvider corLibModuleProvider, TypeSystemContext typeSystemContext)
        {
            _data = data;
            _nameMangler = nameMangler;
            Label = LabelGenerator.GetLabel(LabelType.String);
            _corLibModuleProvider = corLibModuleProvider;
            _typeSystemContext = typeSystemContext;
        }

        public override string Name => "Frozen String Node";

        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            var instructionsBuilder = new InstructionsBuilder();

            var systemStringType = _typeSystemContext.Create(_corLibModuleProvider.FindThrow("System.String"));
            var systemStringEETypeMangledName = _nameMangler.GetMangledTypeName(systemStringType);

            instructionsBuilder.Label(Label);

            instructionsBuilder.Dw(systemStringEETypeMangledName);

            byte lsb = (byte)(_data.Length & 0xFF);
            byte msb = (byte)((_data.Length >> 8) & 0xFF);

            instructionsBuilder.Db(lsb);
            instructionsBuilder.Db(msb);

            foreach (var ch in _data)
            {
                instructionsBuilder.Db((byte)ch);
                instructionsBuilder.Db((byte)0x00);
            }

            return instructionsBuilder.Instructions;
        }
    }
}