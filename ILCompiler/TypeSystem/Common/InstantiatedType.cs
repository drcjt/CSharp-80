using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public class InstantiatedType : MetadataType
    {
        private readonly MetadataType _typeDef;
        private readonly Instantiation _instantiation;

        public InstantiatedType(MetadataType typeDef, Instantiation instantiation)
        {
            _typeDef = typeDef;
            _instantiation = instantiation;
            _baseType = this;
        }

        public override TypeSystemContext Context => _typeDef.Context;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var clone = new TypeDesc[_instantiation.Length];
            for (int i = 0; i < _instantiation.Length; i++)
            {
                var uninstantiated = _instantiation[i];
                var instantiated = uninstantiated.InstantiateSignature(typeInstantiation, methodInstantiation);
                clone[i] = instantiated;
            }

            return Context.GetInstantiatedType(_typeDef, new Instantiation(clone));
        }

        public override ClassLayoutMetadata GetClassLayout()
        {
            return _typeDef.GetClassLayout();
        }

        public override MethodImplRecord[] FindMethodsImplWithMatchingDeclName(string name)
        {
            MethodImplRecord[] uninstMethodImpls = _typeDef.FindMethodsImplWithMatchingDeclName(name);
            return InstantiateMethodImpls(uninstMethodImpls);
        }

        public override Instantiation? Instantiation => _instantiation;

        public override bool IsSequentialLayout => _typeDef.IsSequentialLayout;

        private MethodImplRecord[] InstantiateMethodImpls(MethodImplRecord[] uninstMethodImpls)
        {
            if (uninstMethodImpls.Length == 0)
                return uninstMethodImpls;

            MethodImplRecord[] instMethodImpls = new MethodImplRecord[uninstMethodImpls.Length];

            for (int i = 0; i < uninstMethodImpls.Length; i++)
            {
                MethodDesc decl;

                var implTypeInstantiated = uninstMethodImpls[i].Decl.OwningType.InstantiateSignature(this.Instantiation, default(Instantiation));
                if (implTypeInstantiated is InstantiatedType instantiatedType)
                {
                    decl = _typeDef.Context.GetMethodForInstantiatedType(uninstMethodImpls[i].Decl.GetTypicalMethodDefinition(), instantiatedType);
                }
                else
                {
                    decl = uninstMethodImpls[i].Decl;
                }

                MethodDesc body = _typeDef.Context.GetMethodForInstantiatedType(uninstMethodImpls[i].Body, this);
                instMethodImpls[i] = new MethodImplRecord(decl, body);
            }

            return instMethodImpls;
        }

        public override DefType[] ExplicitlyImplementedInterfaces
        {
            get
            {
                var implementedInterfaces = InstantiateTypeArray(_typeDef.ExplicitlyImplementedInterfaces, _instantiation, default(Instantiation));
                return implementedInterfaces;
            }
        }

        public override MetadataType? MetadataBaseType
        {
            get
            {
                var uninst = _typeDef.BaseType;
                return (uninst != null) ? (MetadataType)uninst.InstantiateSignature(_instantiation, default(Instantiation)) : null;
            }
        }

        public static T[] InstantiateTypeArray<T>(T[] uninstantiatedTypes, Instantiation? typeInstantiation, Instantiation? methodInstantiation) where T : TypeDesc
        {
            T[]? clone = null;

            for (int i = 0; i < uninstantiatedTypes.Length; i++)
            {
                T uninst = uninstantiatedTypes[i];
                TypeDesc inst = uninst.InstantiateSignature(typeInstantiation, methodInstantiation);
                if (inst != uninst)
                {
                    if (clone == null)
                    {
                        clone = new T[uninstantiatedTypes.Length];
                        for (int j = 0; j < clone.Length; j++)
                        {
                            clone[j] = uninstantiatedTypes[j];
                        }
                    }
                    clone[i] = (T)inst;
                }
            }

            return clone ?? uninstantiatedTypes;
        }

        public override TypeDesc GetTypeDefinition()
        {
            return _typeDef;
        }

        public override IEnumerable<FieldDesc> GetFields()
        {
            foreach (var fieldDef in _typeDef.GetFields())
            {
                yield return _typeDef.Context.GetFieldForInstantiatedType(fieldDef, this);
            }
        }

        public override VarType VarType => _typeDef.VarType;

        public override string Name => _typeDef.Name;

        public override bool IsValueType => _typeDef.IsValueType;

        public override bool IsInterface => _typeDef.IsInterface;

        private MetadataType? _baseType;

        private MetadataType? InitializeBaseType()
        {
            var uninstantiatedBaseType = _typeDef.BaseType;
            if (uninstantiatedBaseType != null)
            {
                _baseType = (MetadataType)uninstantiatedBaseType.InstantiateSignature(_instantiation, default(Instantiation));
            }
            else
            {
                _baseType = null;
            }
            return _baseType;
        }

        public override DefType? BaseType
        {
            get
            {
                if (_baseType == this)
                    return InitializeBaseType();
                return _baseType;
            }
        }

        public override IEnumerable<MethodDesc> GetMethods()
        {
            foreach (var typicalMethodDef in _typeDef.GetMethods())
            {
                yield return _typeDef.Context.GetMethodForInstantiatedType(typicalMethodDef, this);
            }
        }

        public override IEnumerable<MethodDesc> GetVirtualMethods()
        {
            foreach (var typicalMethodDef in _typeDef.GetVirtualMethods())
            {
                yield return _typeDef.Context.GetMethodForInstantiatedType(typicalMethodDef, this);
            }
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            var canonInstantiation = Context.ConvertInstantiationToCanonForm(Instantiation!, kind, out bool needsChange);
            if (needsChange)
            {
                MetadataType openType = (MetadataType)GetTypeDefinition();
                return Context.GetInstantiatedType(openType, canonInstantiation);
            }                

            return this;
        }
    }
}