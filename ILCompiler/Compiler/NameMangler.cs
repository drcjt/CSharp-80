using dnlib.DotNet;
using ILCompiler.Interfaces;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class NameMangler : INameMangler
    {
        private Dictionary<MethodDef, String> _mangledMethodNames = new Dictionary<MethodDef, String>();

        private static int nextMethodId = 0;

        public string GetMangledMethodName(MethodDef method)
        {
            string mangledName;
            if (_mangledMethodNames.TryGetValue(method, out mangledName))
                return mangledName;


            mangledName = $"m{nextMethodId++}";
            _mangledMethodNames.Add(method, mangledName);

            return mangledName;
        }
    }
}
