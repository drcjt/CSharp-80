using dnlib.DotNet;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ConstructedEETypeNode : DependencyNode
    {
        public override IList<IDependencyNode> Dependencies { get; set; }

        public ITypeDefOrRef Type { get; private set; }
        public int BaseSize { get; private set; }

        public TypeDef? RelatedType { get; set; }

        public override string Name => Type.FullName + " constructed";

        public ConstructedEETypeNode(ITypeDefOrRef type, int baseSize)
        {
            Type = type;
            BaseSize = baseSize;
            Dependencies = new List<IDependencyNode>();
        }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context)
        {
            var dependencies = new List<IDependencyNode>();
            if (Type.ToTypeSig().IsSZArray)
            {
                var arrayType = context.CorLibModuleProvider.FindThrow("System.Array");

                var allocSize = arrayType.ToTypeSig().GetInstanceByteCount();
                var constructedEETypeNode = context.NodeFactory.ConstructedEETypeNode(arrayType, allocSize);
                dependencies.Add(constructedEETypeNode);
            }
            else
            {
                var baseType = Type.GetBaseType();

                if (baseType != null)
                {
                    var resolvedBaseType = baseType.ResolveTypeDefThrow();
                    RelatedType = resolvedBaseType;

                    var objType = baseType.ToTypeSig();
                    if (!objType.IsValueType)
                    {
                        var allocSize = objType.GetInstanceByteCount();
                        var constructedEETypeNode = context.NodeFactory.ConstructedEETypeNode(resolvedBaseType, allocSize);
                        dependencies.Add(constructedEETypeNode);
                    }
                }
            }
            Dependencies = dependencies;

            return Dependencies;
        }
    }
}