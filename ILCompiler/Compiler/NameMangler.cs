﻿using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class NameMangler : INameMangler
    {
        private readonly Dictionary<String, String> _mangledMethodNames = new Dictionary<String, String>();

        private int nextMethodId = 0;

        private readonly Dictionary<String, String> _mangledFieldNames = new Dictionary<String, String>();

        private int nextFieldId = 0;

        private readonly Dictionary<String, String> _mangledTypeNames = new Dictionary<String, String>();

        private int nextTypeId = 0;


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

        public string GetMangledFieldName(FieldDesc field)
        {
            return GetMangledFieldName(field.FullName);
        }

        public string GetMangledTypeName(TypeDesc type)
        {
            return GetMangledTypeName(type.FullName);
        }

        private string GetMangledTypeName(string fullName)
        {
            if (_mangledTypeNames.TryGetValue(fullName, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"t{nextTypeId++}";
            _mangledTypeNames.Add(fullName, mangledName);

            return mangledName;
        }

        public string GetUniqueName()
        {
            return $"u{nextFieldId++}";
        }

        private string GetMangledFieldName(string fullName)
        {
            if (_mangledFieldNames.TryGetValue(fullName, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"f{nextFieldId++}";
            _mangledFieldNames.Add(fullName, mangledName);

            return mangledName;
        }
    }
}
