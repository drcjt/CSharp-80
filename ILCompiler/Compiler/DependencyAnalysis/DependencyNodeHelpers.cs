namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class DependencyNodeHelpers
    {
        public static IEnumerable<IDependencyNode> GetFlattenedDependencies(IDependencyNode node)
        {
            var flattenedDependencies = new HashSet<IDependencyNode>();
            FlattenDependencies(node, flattenedDependencies);
            return flattenedDependencies;
        }

        private static void FlattenDependencies(IDependencyNode node, ISet<IDependencyNode> flattenedDependencies)
        {
            flattenedDependencies.Add(node);

            foreach (var dependency in node.Dependencies)
            {
                if (!flattenedDependencies.Contains(dependency))
                {
                    FlattenDependencies(dependency, flattenedDependencies);
                }
            }
        }
    }
}
