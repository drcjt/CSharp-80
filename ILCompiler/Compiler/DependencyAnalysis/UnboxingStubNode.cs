using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class UnboxingStubNode : DependencyNode, IMethodNode
    {
        public MethodDesc Method { get; init; }

        private readonly INameMangler _nameMangler;
        public UnboxingStubNode(MethodDesc target, INameMangler nameMangler) 
        {
            Method = target;
            _nameMangler = nameMangler;
        }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context)
        {
            var methodNode = context.NodeFactory.MethodNode(Method);

            return new List<IDependencyNode>() { methodNode };
        }

        public override IList<Instruction> GetInstructions(string inputFilePath)
        {
            // Generate instructions to unbox this parameter and jump to real method
            InstructionsBuilder builder = new InstructionsBuilder();

            builder.Comment($"Assembly listing for method {Name}");
            builder.Label(GetMangledName(_nameMangler));
            //builder.Label(_nameMangler.GetMangledMethodName(Name));

            if (Method.Parameters.Count == 1)
            {
                builder.Pop(BC);    // Return address
                builder.Pop(HL);    // This pointer
                builder.Inc(HL);    // unbox
                builder.Inc(HL);
                builder.Push(HL);   // unboxed this pointer
                builder.Push(BC);   // return address
            }
            else
            {
                int totalParametersByteSize = 0;
                foreach (var parameter in Method.Parameters)
                {
                    var argumentSize = Math.Max(2, parameter.Type.GetElementSize().AsInt);  // Minimum size for parameters on stack is 2 bytes
                    totalParametersByteSize += argumentSize;
                }
                int stackOffsetForThisPtr = totalParametersByteSize;

                builder.Ld(HL, (short)stackOffsetForThisPtr);
                builder.Add(HL, SP);
                builder.Ld(E, __[HL]);
                builder.Inc(HL);
                builder.Ld(D, __[HL]);
                builder.Inc(DE);
                builder.Inc(DE);
                builder.Ld(__[HL], D);
                builder.Dec(HL);
                builder.Ld(__[HL], E);
            }

            var targetMethodMangledMethodName = _nameMangler.GetMangledMethodName(Method);
            builder.Jp(targetMethodMangledMethodName);

            return builder.Instructions;
        }

        public string GetMangledName(INameMangler nameMangler) => "unbox_" + nameMangler.GetMangledMethodName(Method);

        public override string Name => "unbox_" + Method.FullName;
    }
}