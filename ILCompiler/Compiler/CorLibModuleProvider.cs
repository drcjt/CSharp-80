using dnlib.DotNet;

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

            throw new NullReferenceException("System.String type cannot be found in corlib module");
        }
    }
}
