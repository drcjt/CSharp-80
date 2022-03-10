using dnlib.DotNet;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class NameMangler : INameMangler
    {
        private readonly Dictionary<String, String> _mangledMethodNames = new Dictionary<String, String>();

        private static int nextMethodId = 0;

        public string GetMangledMethodName(MethodDef method)
        {
            if (_mangledMethodNames.TryGetValue(method.FullName, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"m{nextMethodId++}";
            _mangledMethodNames.Add(method.FullName, mangledName);

            return mangledName;
        }
    }
}
