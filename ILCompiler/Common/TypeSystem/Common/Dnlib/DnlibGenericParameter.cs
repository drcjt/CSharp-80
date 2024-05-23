using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common.dnlib
{
    public sealed class DnlibGenericParameter : GenericParameterDesc
    {
        private TypeSystemContext _context;
        private GenericParam _genericParameter;
        public DnlibGenericParameter(TypeSystemContext context, GenericParam genericParameter)
        {
            _context = context;
            _genericParameter = genericParameter;
        }

        public override int Index => (int)_genericParameter.Number;

        public override GenericParameterKind Kind => _genericParameter.Owner.IsGenericParam ? GenericParameterKind.Type : GenericParameterKind.Method;

        public override TypeSystemEntity AssociatedTypeOrMethod => Context.CreateFromTypeOrMethodDef(_genericParameter.Owner);

        public override TypeSystemContext Context => _context;
    }
}