namespace ILCompiler.Common.TypeSystem.Common
{
    public abstract class ModuleDesc : TypeSystemEntity
    {
        public override TypeSystemContext Context { get; }

        public ModuleDesc(TypeSystemContext context)
        {
            Context = context;
        }

        public abstract object GetType(string nameSpace, string name);
    }
}
