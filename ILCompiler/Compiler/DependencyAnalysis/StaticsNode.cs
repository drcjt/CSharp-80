using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class StaticsNode : DependencyNode
    {
        public FieldDesc Field { get; private set; }

        public override string Name => Field.FullName;

        private readonly PreinitializationManager _preinitializationManager;
        private readonly INameMangler _nameMangler;

        public StaticsNode(FieldDesc field, PreinitializationManager preinitializationManager, INameMangler nameMangler)
        {
            Field = field;
            _preinitializationManager = preinitializationManager;
            _nameMangler = nameMangler;
        }

        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            var instructionsBuilder = new InstructionsBuilder();

            var field = Field;

            if (_preinitializationManager.IsPreinitialized(field.OwningType))
            {
                var preinitializationInfo = _preinitializationManager.GetPreinitializationInfo(field.OwningType);
                var value = preinitializationInfo.GetFieldValue(field);

                // Need to mangle full field name here
                instructionsBuilder.Label(_nameMangler.GetMangledFieldName(field));

                var bytes = value.GetRawData();
                foreach (var b in bytes)
                {
                    instructionsBuilder.Db(b);
                }
            }
            else
            {
                var fieldSize = field.FieldType.GetElementSize().AsInt;
                instructionsBuilder.Comment($"Reserving {fieldSize} bytes for static field {field.FullName}");

                // Need to mangle full field name here
                instructionsBuilder.Label(_nameMangler.GetMangledFieldName(field));

                // Emit fieldSize bytes with value 0
                instructionsBuilder.Dc((ushort)fieldSize, 0);
            }

            return instructionsBuilder.Instructions;
        }
    }
}
