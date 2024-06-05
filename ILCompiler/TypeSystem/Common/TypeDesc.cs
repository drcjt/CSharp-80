using ILCompiler.Compiler;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class TypeDesc : TypeSystemEntity
    {
        public virtual string FullName => String.Empty;
        public virtual string Name => String.Empty;
        public virtual string Namespace => String.Empty;
        public bool IsDefType => this is DefType;
        public bool IsParameterizedType => this is ParameterizedType;
        public bool IsFunctionPointer => this is FunctionPointerType;
        public bool IsPointer => this is PointerType;
        public bool IsByRef => false;

        public bool IsInstantiatedType => this is InstantiatedType;

        public virtual bool IsValueType => false;

        public virtual DefType? BaseType => null;
        public bool HasBaseType => BaseType != null;

        public virtual bool HasStaticConstructor => false;

        public virtual bool IsEnum => false;
        public virtual bool IsPrimitive => false;

        public virtual bool IsInterface {  get; }

        public bool IsArray => this is ArrayType;
        public bool IsSzArray => this is ArrayType type && type.IsSzArray;

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
                return MetadataRuntimeInterfacesAlgorithm.ComputeRuntimeInterfaces(this);
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

        public virtual TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation methodInstantiation) => this;
    }
}
