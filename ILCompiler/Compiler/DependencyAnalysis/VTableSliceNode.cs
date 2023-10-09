using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class VTableSliceNode : DependencyNode
    {
        private readonly TypeDef _type;
        public VTableSliceNode(TypeDef type)
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
        public IReadOnlyList<MethodDef> GetSlots()
        {
            IList<MethodDef> slots = new List<MethodDef>();

            foreach (var method in _type.Methods)
            {
                if (_usedMethods.Contains(method.FullName))
                {
                    slots.Add(method);
                }
            }

            return slots.ToArray();
        }
    }
}
