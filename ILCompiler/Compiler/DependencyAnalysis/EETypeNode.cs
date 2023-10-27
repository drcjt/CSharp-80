using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class EETypeNode : DependencyNode
    {
        public ITypeDefOrRef Type { get; private set; }

        public override string Name => Type.FullName;

        protected readonly INameMangler _nameMangler;

        public EETypeNode(ITypeDefOrRef type, INameMangler nameMangler)
        {
            Type = type;
            _nameMangler = nameMangler;
            _eeTypePtr = _nextEETypePtr++;
        }

        public string MangledTypeName => _nameMangler.GetMangledTypeName(Type);

        private static ushort _nextEETypePtr = 1;

        private readonly ushort _eeTypePtr;
        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            var instructionsBuilder = new InstructionsBuilder();
            instructionsBuilder.Comment($"{Type.FullName}");

            instructionsBuilder.Equ(MangledTypeName, _eeTypePtr);

            return instructionsBuilder.Instructions;
        }
    }
}