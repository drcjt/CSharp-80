using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class StaticsNode : DependencyNode
    {
        private readonly MetadataType _type;

        public override string Name => $"StaticsNode_{_type.ToString()}";

        private readonly PreinitializationManager _preinitializationManager;
        private readonly INameMangler _nameMangler;

        public StaticsNode(MetadataType type, PreinitializationManager preinitializationManager, INameMangler nameMangler)
        {
            _type = type;
            _preinitializationManager = preinitializationManager;
            _nameMangler = nameMangler;
        }

        public override IList<Instruction> GetInstructions(string inputFilePath, IList<string> modules)
        {
            var instructionsBuilder = new InstructionsBuilder();

            instructionsBuilder.Comment($"Bytes for static fields for type {_type.ToString()}");
            instructionsBuilder.Label(_nameMangler.GetMangledTypeName(_type) + "_" + "statics");

            if (_preinitializationManager.IsPreinitialized(_type))
            {
                BuildInstructionsForPreinitializedStatics(instructionsBuilder);
            }
            else
            {
                var staticsSize = _type.StaticFieldSize.AsInt;

                foreach (var field in _type.GetFields())
                {
                    if (!field.IsStatic || field.IsLiteral)
                        continue;

                    instructionsBuilder.Comment($"Field {field.Name} offset: {field.Offset.AsInt}");
                }

                instructionsBuilder.Dc((ushort)staticsSize, 0);
            }

            return instructionsBuilder.Instructions;
        }

        private void BuildInstructionsForPreinitializedStatics(InstructionsBuilder instructionsBuilder)
        {
            var preinitializationInfo = _preinitializationManager.GetPreinitializationInfo(_type);

            int byteCount = 0;
            foreach (var field in _type.GetFields())
            {
                if (!field.IsStatic || field.IsLiteral)
                    continue;

                instructionsBuilder.Comment($"Field {field.Name} offset: {field.Offset.AsInt}");

                int padding = field.Offset.AsInt - byteCount;
                instructionsBuilder.Dc((ushort)padding, 0);

                var value = preinitializationInfo.GetFieldValue(field);
                var bytes = value.GetRawData();
                foreach (var b in bytes)
                {
                    instructionsBuilder.Db(b);
                    byteCount++;
                }
            }
        }
    }
}