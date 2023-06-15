using dnlib.DotNet;
using System.Runtime.Serialization;

namespace ILCompiler.Compiler
{
    public class CorLibModuleProvider
    {
        public ModuleDefMD? CorLibModule { get; set; }
        public TypeDef FindThrow(string name)
        {
            var typeDef = CorLibModule?.Find(name, false);
            if (typeDef is not null)
            {
                return typeDef;
            }

            throw new CorLibTypeResolutionException("System.String type cannot be found in corlib module");
        }
    }

    [Serializable]
    public class CorLibTypeResolutionException : Exception
    {
        public CorLibTypeResolutionException() { }
        public CorLibTypeResolutionException(string message) : base(message) { }
        public CorLibTypeResolutionException(string message, Exception innerException) : base(message, innerException) { }
        public CorLibTypeResolutionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
