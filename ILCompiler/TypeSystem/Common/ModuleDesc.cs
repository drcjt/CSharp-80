namespace ILCompiler.TypeSystem.Common
{
    public abstract class ModuleDesc : TypeSystemEntity
    {
        public override TypeSystemContext Context { get; }

        protected ModuleDesc(TypeSystemContext context)
        {
            Context = context;
        }

        public abstract MetadataType GetType(string nameSpace, string name);
    }
}
