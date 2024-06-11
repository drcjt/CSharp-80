using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.Dnlib
{
    public sealed class DnlibType : MetadataType
    {
        private readonly TypeDef _typeDef;
        private readonly DnlibModule _module;
        public DnlibType(TypeDef typeDef, DnlibModule module)
        {
            _typeDef = typeDef;
            _module = module;
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
                _baseType = (MetadataType)_module.Create(_typeDef.BaseType);
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

        public override TypeSystemContext Context => _module.Context;


        public override bool IsSequentialLayout => _typeDef.IsSequentialLayout;

        public override string FullName => _typeDef.FullName;
        public override string Name => _typeDef.Name;
        public override string Namespace => _typeDef.Namespace;

        public override bool IsInterface => _typeDef.IsInterface;

        public override bool IsEnum => _typeDef.IsEnum;
        public override bool IsPrimitive => _typeDef.IsPrimitive;
        public override bool IsValueType => _typeDef.IsValueType;


        public override VarType VarType => GetVarType(_typeDef.ToTypeSig());


        public override MethodDesc? GetStaticConstructor()
        {
            var methodDef = _typeDef.FindStaticConstructor();
            return methodDef == null ? null : _module.Create(methodDef);
        }

        public override MethodDesc? GetDefaultConstructor()
        {
            var methodDef = _typeDef.FindDefaultConstructor();
            return methodDef == null ? null : _module.Create(methodDef);
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

                return _typeDef.Interfaces.Select(x => (DefType)(_module.Create(x.Interface))).ToArray();
            }
        }

        public override IEnumerable<MethodDesc> GetMethods()
        {
            foreach (var method in _typeDef.Methods)
            {
                yield return _module.Create(method);
            }
        }

        public override IEnumerable<FieldDesc> GetFields()
        {
            foreach (var field in _typeDef.Fields)
            {
                yield return _module.Create(field);
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

        public override MethodImplRecord[] FindMethodsImplWithMatchingDeclName(string name)
        {
            var foundRecords = new List<MethodImplRecord>();
            foreach (var method in GetVirtualMethods())
            {
                foreach (var methodOverride in method.Overrides.Where(x => x.MethodDeclaration.Name == name))
                {
                    var newRecord = new MethodImplRecord(_module.Create(methodOverride.MethodDeclaration), _module.Create(methodOverride.MethodBody));
                    foundRecords.Add(newRecord);
                }
            }

            return foundRecords.ToArray();
        }

        private static VarType GetVarType(TypeSig typeSig)
        {
            var typeDefOrRef = typeSig.TryGetTypeDefOrRef();
            var typeDef = typeDefOrRef.ResolveTypeDef();

            if (typeDef != null && typeDef.IsEnum)
            {
                return GetVarType(typeDef.GetEnumUnderlyingType());
            }

            switch (typeSig.ElementType)
            {
                case ElementType.Void:
                    return VarType.Void;

                case ElementType.Boolean:
                    return VarType.Byte;

                case ElementType.I1:
                    return VarType.SByte;
                case ElementType.U1:
                    return VarType.Byte;

                case ElementType.I2:
                    return VarType.Short;
                case ElementType.U2:
                case ElementType.Char:
                    return VarType.UShort;

                case ElementType.I4:
                    return VarType.Int;
                case ElementType.U4:
                    return VarType.UInt;

                case ElementType.Ptr:
                case ElementType.I:
                case ElementType.U:
                    return VarType.Ptr;

                case ElementType.ValueType:
                    return VarType.Struct;

                case ElementType.Class:
                case ElementType.String:
                case ElementType.Array:
                case ElementType.SZArray:
                case ElementType.Object:
                    return VarType.Ref;

                case ElementType.Pinned:
                    return GetVarType(typeSig.Next);

                case ElementType.ByRef:
                    return VarType.ByRef;

                default:
                    throw new NotSupportedException($"ElementType : {typeSig.ElementType} cannot be converted to VarType");
            }
        }
    }
}