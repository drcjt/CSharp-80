﻿using dnlib.DotNet;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Common
{
    internal class MethodForInstantiatedType : MethodDesc
    {
        private readonly MethodDesc _typicalMethodDef;
        private readonly InstantiatedType _instantiatedType;

        public MethodForInstantiatedType(MethodDesc typicalMethodDef, InstantiatedType instantiatedType)
        {
            _typicalMethodDef = typicalMethodDef;
            _instantiatedType = instantiatedType;
        }

        public override TypeSystemContext Context => _typicalMethodDef.Context;

        public override TypeDesc OwningType => _instantiatedType;

        public override MethodSignature Signature
        {
            get
            {
                var template = _typicalMethodDef.Signature;
                var builder = new MethodSignatureBuilder(template);

                builder.ReturnType = Instantiate(template.ReturnType);
                for (int i = 0; i < template.Length; i++)
                {
                    builder[i] = Instantiate(template[i].Type);
                }

                return builder.ToSignature();
            }
        }

        public override IList<MethodParameter> Parameters => _typicalMethodDef.Parameters;

        public override IList<LocalVariableDefinition> Locals
        {
            get
            {
                var instantiatedLocals = new List<LocalVariableDefinition>();
                foreach (var local in _typicalMethodDef.Locals)
                {
                    var instantiatedLocal = new LocalVariableDefinition(Instantiate(local.Type), local.Name, local.Index);
                    instantiatedLocals.Add(instantiatedLocal);
                }
                return instantiatedLocals;
            }
        }

        public override MethodIL? MethodIL => _typicalMethodDef.MethodIL;


        public override Instantiation Instantiation => _typicalMethodDef.Instantiation;

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName)
        {
            return _typicalMethodDef.HasCustomAttribute(attributeNamespace, attributeName);
        }

        private TypeDesc Instantiate(TypeDesc type) => type.InstantiateSignature(_instantiatedType.Instantiation, null);

        // TODO: Refactor to not use dnlib types in following
        public override IList<MethodOverride> Overrides => throw new NotImplementedException();
        public override MethodSig MethodSig => throw new NotImplementedException();
        public override CustomAttributeCollection CustomAttributes => throw new NotImplementedException();
        public override string Name => _typicalMethodDef.Name;
        public override string FullName => ToString();
        public override bool HasReturnType => _typicalMethodDef.HasReturnType;
    }
}