using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Utilities;
using System.Text;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class TypeDesc : TypeSystemEntity
    {
        public static readonly TypeDesc[] EmptyTypes = [];
        public string FullName
        {
            get
            {
                var typeNameFormatter = new TypeNameFormatter();
                StringBuilder sb = new StringBuilder();
                typeNameFormatter.AppendName(sb, this);
                return sb.ToString();
            }
        }

        public virtual string Name => String.Empty;
        public virtual string Namespace => String.Empty;
        public bool IsDefType => this is DefType;
        public bool IsParameterizedType => this is ParameterizedType;
        public bool IsFunctionPointer => this is FunctionPointerType;
        public bool IsPointer => this is PointerType;
        public bool IsByRef => false;
        public bool IsObject => this.IsWellKnownType(WellKnownType.Object);

        public bool IsInstantiatedType => this is InstantiatedType;

        // TODO: Does this need to be nullable now we have Instantiation.Empty?
        public virtual Instantiation? Instantiation => Instantiation.Empty;

        public bool HasInstantiation => Instantiation?.Length != 0;

        public virtual bool IsValueType => false;

        public virtual DefType? BaseType => null;
        public bool HasBaseType => BaseType != null;

        public virtual bool HasStaticConstructor => false;

        public virtual bool IsEnum => false;
        public virtual bool IsPrimitive => false;

        public virtual bool IsInterface {  get; }

        public bool IsArray => this is ArrayType;

        public bool IsMdArray => this is ArrayType type && type.IsMdArray;
        public bool IsSzArray => this is ArrayType type && type.IsSzArray;

        public bool IsString => this.IsWellKnownType(WellKnownType.String);

        public bool IsVoid => IsVarType(VarType.Void);

        public virtual IEnumerable<FieldDesc> GetFields() => FieldDesc.EmptyFields;

        public virtual TypeDesc UnderlyingType
        {
            get
            {
                if (!this.IsEnum)
                {
                    return this;
                }

                foreach (var field in this.GetFields())
                {
                    if (!field.IsStatic)
                        return field.FieldType;
                }

                throw new InvalidOperationException();
            }
        }

        public virtual TypeFlags Category { get; }

        public virtual MethodDesc? GetStaticConstructor() => null;
        public virtual MethodDesc? GetDefaultConstructor() => null;

        public bool IsVarType(VarType varType) => varType == VarType;

        public virtual VarType VarType { get; } = VarType.Void;

        public DefType[] RuntimeInterfaces
        {
            get
            {
                if (this.IsDefType)
                {
                    return MetadataRuntimeInterfacesAlgorithm.ComputeRuntimeInterfaces(this);
                }
                else if (this.IsArray)
                {
                    ArrayType arrayType = (ArrayType)this;
                    var elementType = arrayType.ElementType;
                    if (arrayType.IsSzArray && !elementType.IsPointer && !elementType.IsFunctionPointer)
                    {
                        return ArrayOfTRuntimeInterfacesAlgorithm.ComputeRuntimeInterfaces(this);
                    }
                    else
                    {
                        // TODO: multi-dimensional arrays, and arrays of pointers or function pointers
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public virtual IEnumerable<MethodDesc> GetVirtualMethods()
        {
            foreach (MethodDesc method in GetMethods())
                if (method.IsVirtual)
                    yield return method;
        }

        public virtual IEnumerable<MethodDesc> GetMethods() => new MethodDesc[0];

        public MethodDesc? FindMethodEndsWith(string name)
        {
            foreach (var method in GetMethods())
            {
                if (method.FullName.EndsWith(name))
                {
                    return method;
                }
            }

            return null;
        }

        public virtual TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation) => this;

        public virtual TypeDesc GetTypeDefinition() => this;

        public override string ToString()
        {
            var sb = new StringBuilder();
            var typeNameFormatter = new TypeNameFormatter();
            typeNameFormatter.AppendName(sb, this);
            return sb.ToString();
        }

        public TypeDesc ConvertToCanonForm(CanonicalFormKind kind) => ConvertToCanonFormImpl(kind);

        protected abstract TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind);

        public bool IsSignatureVariable => this is SignatureTypeVariable || this is SignatureMethodVariable;

        public abstract bool IsCanonicalSubtype(CanonicalFormKind policy);

        public bool IsCanonicalType
        {
            get
            {
                if (Context.IsCanonicalDefinitionType(this, CanonicalFormKind.Any))
                    return true;
                else if (this.IsValueType)
                    return this.IsCanonicalSubtype(CanonicalFormKind.Any);
                else
                    return false;
            }
        }

        public abstract bool IsRuntimeDeterminedSubtype { get; }

        public bool IsGenericDefinition => HasInstantiation && IsTypeDefinition;

        public bool IsTypeDefinition => GetTypeDefinition() == this;
    }
}
