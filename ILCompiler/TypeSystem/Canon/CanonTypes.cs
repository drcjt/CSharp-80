using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Common;
using System.Diagnostics;

namespace ILCompiler.TypeSystem.Canon
{
    public enum CanonicalFormKind
    {
        Specific,
        Any,
    }

    public abstract partial class CanonBaseType : MetadataType
    {
        private readonly TypeSystemContext _context;

        public CanonBaseType(TypeSystemContext context)
        {
            _context = context;
        }

        public sealed override TypeSystemContext Context => _context;

        public override MetadataType? MetadataBaseType => BaseType as MetadataType;

        public override DefType[] ExplicitlyImplementedInterfaces => Array.Empty<DefType>();

        public override bool IsSequentialLayout => false;

        public override MethodImplRecord[] FindMethodsImplWithMatchingDeclName(string name) => Array.Empty<MethodImplRecord>();

        public override ClassLayoutMetadata GetClassLayout() => default;

        public override VarType VarType => VarType.Ref;
    }

    internal sealed class CanonType : CanonBaseType
    {
        public new string FullName => Namespace + "." + Name;
        public override string Namespace => "System";
        public override string Name => "__Canon";

        public CanonType(TypeSystemContext context) : base(context)
        {
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            Debug.Assert(kind == CanonicalFormKind.Specific);
            return this;
        }
    }
}
