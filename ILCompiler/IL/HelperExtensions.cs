using ILCompiler.TypeSystem.Common;

namespace ILCompiler.IL
{
    internal static class HelperExtensions
    {
        private const string HelperTypesNamespace = "Internal.Runtime.CompilerHelpers";

        public static MetadataType GetKnownType(this ModuleDesc module, string @namespace, string name)
        {
            MetadataType type = (MetadataType)module.GetType(@namespace, name);
            if (type == null)
            {
                var fullyQualifiedName = @namespace.Length > 0 ? @namespace + "." + name : name;
                throw new InvalidOperationException($"Expected type '{fullyQualifiedName}' not found in module '{module}'");
            }
            return type;
        }

        public static MethodDesc GetKnownMethod(this TypeDesc type, string name)
        {
            var method = type.GetMethods().First(x => x.Name == name);
            if (method == null)
            {
                throw new InvalidOperationException($"Expected method '{name}' not found on type '{type}");
            }
            return method;
        }

        public static MethodDesc GetHelperEntryPoint(this TypeSystemContext context, string typeName, string methodName)
        {
            TypeDesc helperType = (TypeDesc)context.SystemModule!.GetType(HelperTypesNamespace, typeName);
            return helperType.GetMethods().First(x => x.Name == methodName);
        }

        public static MethodDesc GetHelperEntryPoint(this TypeSystemContext context, string typeNamespace, string typeName, string methodName)
        {
            TypeDesc helperType = (TypeDesc)context.SystemModule!.GetType(typeNamespace, typeName);
            return helperType.GetMethods().First(x => x.Name == methodName);
        }
    }
}