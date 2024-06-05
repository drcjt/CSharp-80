using dnlib.DotNet;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.Dnlib
{
    public sealed class DnlibGenericParameter : GenericParameterDesc
    {
        private readonly DnlibModule _module;
        private readonly GenericParam _genericParameter;
        public DnlibGenericParameter(DnlibModule module, GenericParam genericParameter)
        {
            _module = module;
            _genericParameter = genericParameter;
        }

        public override int Index => (int)_genericParameter.Number;

        public override GenericParameterKind Kind => _genericParameter.Owner.IsGenericParam ? GenericParameterKind.Type : GenericParameterKind.Method;

        public override TypeSystemEntity AssociatedTypeOrMethod => _module.CreateFromTypeOrMethodDef(_genericParameter.Owner);

        public override TypeSystemContext Context => _module.Context;
    }
}