using dnlib.DotNet;
using ILCompiler.Compiler;

namespace ILCompiler.Common.TypeSystem.Common.dnlib
{
    public sealed class DnlibType : MetadataType
    {
        private readonly TypeDef _typeDef;
        private readonly TypeSystemContext _typeSystemContext;
        public DnlibType(TypeDef typeDef, TypeSystemContext typeSystemContext)
        {
            _typeDef = typeDef;
            _typeSystemContext = typeSystemContext;
            _baseType = this;
        }

        private MetadataType? _baseType;
        private MetadataType? InitializeBaseType()
        {
            if (_typeDef.BaseType is null)
            {
                _baseType = null;
            }
            else
            {
                _baseType = (MetadataType)Context.Create(_typeDef.BaseType);
            }
            return _baseType;
        }

        public override DefType? BaseType
        {
            get
            {
                if (_baseType == this)
                    InitializeBaseType();
                return _baseType;
            }
        }

        public override MetadataType? MetadataBaseType
        {
            get
            {
                if (_baseType == this)
                    InitializeBaseType();
                return _baseType;
            }
        }

        public override bool HasStaticConstructor => _typeDef.FindStaticConstructor() != null;

        public override TypeSystemContext Context => _typeSystemContext;


        public override bool IsSequentialLayout => _typeDef.IsSequentialLayout;

        public override string FullName => _typeDef.FullName;
        public override string Name => _typeDef.Name;
        public override string Namespace => _typeDef.Namespace;

        public override bool IsInterface => _typeDef.IsInterface;

        public override bool IsEnum => _typeDef.IsEnum;
        public override bool IsPrimitive => _typeDef.IsPrimitive;
        public override bool IsValueType => _typeDef.IsValueType;


        public override VarType VarType => _typeDef.ToTypeSig().GetVarType();


        public override MethodDesc? GetStaticConstructor()
        {
            var methodDef = _typeDef.FindStaticConstructor();
            return methodDef == null ? null : Context.Create(methodDef);
        }

        public override MethodDesc? GetDefaultConstructor()
        {
            var methodDef = _typeDef.FindDefaultConstructor();
            return methodDef == null ? null : Context.Create(methodDef);
        }

        public override ClassLayoutMetadata GetClassLayout()
        {
            var layout = new ClassLayoutMetadata();
            if (_typeDef != null && _typeDef.ClassLayout != null)
            {
                layout.PackingSize = _typeDef.ClassLayout.PackingSize;
                layout.Size = (int)_typeDef.ClassLayout.ClassSize;
            }

            return layout;
        }

        public override DefType[] ExplicitlyImplementedInterfaces
        {
            get
            {
                int count = _typeDef.Interfaces.Count;
                if (count == 0)
                    return Array.Empty<DefType>();

                return _typeDef.Interfaces.Select(x => (DefType)(Context.Create(x.Interface))).ToArray();
            }
        }

        public override IEnumerable<MethodDesc> GetMethods()
        {
            foreach (var method in _typeDef.Methods)
            {
                yield return Context.Create(method);
            }
        }

        public override IEnumerable<FieldDesc> GetFields()
        {
            foreach (var field in _typeDef.Fields)
            {
                yield return Context.Create(field);
            }
        }

        public override TypeFlags Category
        {
            get
            {
                return _typeDef.ToTypeSig().ElementType switch
                {
                    ElementType.Void => TypeFlags.Void,
                    ElementType.Boolean => TypeFlags.Boolean,
                    ElementType.Char => TypeFlags.Char,
                    ElementType.I1 => TypeFlags.SByte,
                    ElementType.I2 => TypeFlags.Int16,
                    ElementType.I4 => TypeFlags.Int32,
                    ElementType.U1 => TypeFlags.Byte,
                    ElementType.U2 => TypeFlags.UInt16,
                    ElementType.U4 => TypeFlags.UInt32,
                    ElementType.I => TypeFlags.IntPtr,
                    ElementType.U => TypeFlags.UIntPtr,
                    ElementType.Ptr => TypeFlags.IntPtr,
                    ElementType.ByRef => TypeFlags.ByRef,
                    ElementType.Array => TypeFlags.Array,
                    ElementType.SZArray => TypeFlags.SzArray,
                    _ => TypeFlags.Unknown
                };
            }
        }
    }
}
