using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.RuntimeDetermined
{
    internal class MethodForRuntimeDeterminedType : MethodDesc
    {
        private readonly MethodDesc _typicalMethodDef;
        private readonly RuntimeDeterminedType _rdType;

        public MethodForRuntimeDeterminedType(MethodDesc typicalMethodDef, RuntimeDeterminedType rdType)
        {
            _typicalMethodDef = typicalMethodDef;
            _rdType = rdType;
        }

        public override bool IsVirtual => _typicalMethodDef.IsVirtual;
        public override bool IsNewSlot => _typicalMethodDef.IsNewSlot;
        public override bool IsAbstract => _typicalMethodDef.IsAbstract;

        public override string Name => _typicalMethodDef.Name;

        public override MethodDesc GetTypicalMethodDefinition() => _typicalMethodDef;

        public override Instantiation Instantiation => _typicalMethodDef.Instantiation;

        public override IList<MethodParameter> Parameters => throw new NotImplementedException();

        public override IList<LocalVariableDefinition> Locals => throw new NotImplementedException();

        public override TypeDesc OwningType => _rdType;

        public override MethodIL? MethodIL => throw new NotImplementedException();

        public override MethodSignature Signature => _typicalMethodDef.Signature;


        public override IEnumerable<MethodImplRecord> Overrides => throw new NotImplementedException();

        public override TypeSystemContext Context => _typicalMethodDef.Context;

        public override MethodDesc CreateUserMethod(string name)
        {
            throw new NotImplementedException();
        }

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName)
        {
            return _typicalMethodDef.HasCustomAttribute(attributeNamespace, attributeName);
        }
    }
}
