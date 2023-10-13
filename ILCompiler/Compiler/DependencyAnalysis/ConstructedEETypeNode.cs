using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class ConstructedEETypeNode : DependencyNode
    {
        public ITypeDefOrRef Type { get; private set; }
        public int BaseSize { get; private set; }

        public TypeDef? RelatedType { get; set; }

        public override string Name => Type.FullName + " constructed";

        public ConstructedEETypeNode(ITypeDefOrRef type, int baseSize)
        {
            Type = type;
            BaseSize = baseSize;
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

            return dependencies;
        }

        public override IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context)
        {
            var resolvedType = Type.ResolveTypeDefThrow();

            IList<ConditionalDependency> conditionalDependencies = new List<ConditionalDependency>();
            foreach (var method in VirtualMethodAlgorithm.EnumAllVirtualSlots(resolvedType))
            {
                if (!method.HasGenericParameters)
                {
                    var implementation = VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(resolvedType, method);

                    // Add a conditional dependency if the method implementation is on this type
                    // and is not abstract since there is no code for an abstract method
                    if (implementation?.DeclaringType == resolvedType && !implementation.IsAbstract)
                    {
                        var conditionalDependency = new ConditionalDependency
                        {
                            IfNode = context.NodeFactory.VirtualMethodUse(method),
                            ThenParent = this,
                            ThenNode = context.NodeFactory.MethodNode(implementation),
                        };
                        conditionalDependencies.Add(conditionalDependency);
                    }
                }
                else
                {
                    throw new NotImplementedException("Generic virtual methods not supported");
                }
            }

            return conditionalDependencies;
        }
    }
}