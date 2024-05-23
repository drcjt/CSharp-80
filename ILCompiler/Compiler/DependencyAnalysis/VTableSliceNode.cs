using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class VTableSliceNode : DependencyNode
    {
        private readonly TypeDesc _type;
        public VTableSliceNode(TypeDesc type)
        {
            _type = type;
        }

        public override string Name => _type.FullName + " VTable";

        private readonly ISet<string> _usedMethods = new HashSet<string>();

        public void AddEntry(MethodDesc method)
        {
            _usedMethods.Add(method.FullName);
        }

        /// <summary>
        /// Sort the used virtual methods in metadata order to get the slots
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<MethodDesc> GetSlots()
        {
            IList<MethodDesc> slots = new List<MethodDesc>();
            foreach (var method in _type.GetVirtualMethods().Where(method => _usedMethods.Contains(method.FullName)))
            {
                slots.Add(method);
            }

            return slots.ToArray();
        }
    }
}
