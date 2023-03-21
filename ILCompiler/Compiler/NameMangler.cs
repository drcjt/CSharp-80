using dnlib.DotNet;
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


        public string GetMangledMethodName(MethodSpec method)
        {
            return GetMangledMethodName(method.FullName);
        }

        public string GetMangledMethodName(MethodSpec calleeMethod, MethodDesc callerMethod)
        {
            var calleeMethodDef = calleeMethod.Method.ResolveMethodDefThrow();

            IList<TypeSig> callerMethodGenericParameters = new List<TypeSig>();
            if (callerMethod is InstantiatedMethod method)
            {
                callerMethodGenericParameters = method.GenericParameters;
            }

            var resolvedGenericParameters = new List<TypeSig>();
            foreach (var genericParameter in ((MethodSpec)calleeMethod).GenericInstMethodSig.GenericArguments)
            {
                resolvedGenericParameters.Add(GenericTypeInstantiator.Instantiate(genericParameter, callerMethodGenericParameters));
            }

            var fullMethodName = FullNameFactory.MethodFullName(calleeMethodDef.DeclaringType?.FullName, calleeMethodDef.Name, calleeMethodDef.MethodSig, null, resolvedGenericParameters);
            return GetMangledMethodName(fullMethodName);
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

        public string GetMangledFieldName(FieldDef field)
        {
            return GetMangledFieldName(field.FullName);
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
