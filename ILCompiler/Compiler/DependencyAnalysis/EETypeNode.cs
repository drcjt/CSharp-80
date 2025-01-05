using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class EETypeNode : DependencyNode
    {
        public TypeDesc Type { get; private set; }

        public override string Name => Type.FullName;

        protected readonly INameMangler _nameMangler;

        public EETypeNode(TypeDesc type, INameMangler nameMangler)
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

        public override bool ShouldSkipEmitting(NodeFactory factory)
        {
            // Skip emitting if there is a constructed version of this node
            return factory.ConstructedEETypeNode(this.Type).Mark;
        }
    }
}