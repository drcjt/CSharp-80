using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class NameMangler : INameMangler
    {
        private readonly Dictionary<String, String> _mangledMethodNames = new Dictionary<String, String>();

        private static int nextMethodId = 0;

        public string GetMangledMethodName(MethodSpec method)
        {
            return GetMangledMethodName(method.FullName);
        }

        public string GetMangledMethodName(MethodDef method)
        {
            return GetMangledMethodName(method.FullName);
        }

        public string GetMangledMethodName(MethodDesc method)
        {
            return GetMangledMethodName(method.FullName);
        }

        private string GetMangledMethodName(string fullName)
        {
            if (_mangledMethodNames.TryGetValue(fullName, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"m{nextMethodId++}";
            _mangledMethodNames.Add(fullName, mangledName);

            return mangledName;
        }
    }
}
