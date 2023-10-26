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
        }

        public string MangledTypeName => _nameMangler.GetMangledTypeName(Type);

        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            var instructionsBuilder = new InstructionsBuilder();
            instructionsBuilder.Comment($"{Type.FullName}");

            // Need to mangle full field name here
            instructionsBuilder.Label(MangledTypeName);

            // Need something here to ensure the label has a unique value
            instructionsBuilder.Db(0);

            return instructionsBuilder.Instructions;
        }
    }
}