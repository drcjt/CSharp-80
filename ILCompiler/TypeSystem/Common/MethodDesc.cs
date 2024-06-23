using ILCompiler.TypeSystem.IL;
using System.Text;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class MethodDesc : TypeSystemEntity
    {
        public bool LocallocUsed { get; set; } = false;

        public virtual bool IsIntrinsic => false;

        public virtual bool IsPInvoke => false;

        public virtual string PInvokeMethodName => String.Empty;

        public virtual bool IsInternalCall => false;

        public virtual bool IsStaticConstructor => OwningType.GetStaticConstructor() == this;

        public virtual bool IsDefaultConstructor => OwningType.GetDefaultConstructor() == this;

        public virtual bool IsStatic => false;

        public abstract IList<MethodParameter> Parameters { get; }

        public abstract IList<LocalVariableDefinition> Locals { get; }

        public virtual bool HasReturnType => false;

        public virtual bool HasThis => false;

        public virtual string FullName => String.Empty;

        public abstract TypeDesc OwningType { get; }

        public virtual string Name => string.Empty;

        public abstract MethodIL? MethodIL { get; }

        public abstract bool HasCustomAttribute(string attributeNamespace, string attributeName);

        public virtual bool IsVirtual => false;
        public virtual bool IsNewSlot => false;

        public virtual bool IsAbstract => false;

        public virtual bool HasGenericParameters => false; 

        public abstract MethodSignature Signature { get; }

        public abstract Instantiation Instantiation { get; }

        public virtual MethodDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var clone = new TypeDesc[Instantiation.Length];
            var instantiation = Instantiation;
            for (int i = 0; i < instantiation.Length; i++)
            {
                var uninstantiated = instantiation[i];
                var instantiated = uninstantiated.InstantiateSignature(typeInstantiation, methodInstantiation);

                clone[i] = instantiated;
            }

            var owningType = OwningType;
            var instantiatedOwningType = owningType.InstantiateSignature(typeInstantiation, methodInstantiation);

            if (instantiatedOwningType is InstantiatedType)
            {
                return Context.GetMethodForInstantiatedType(this.GetMethodDefinition(), (InstantiatedType)instantiatedOwningType);
            }
            
            return Context.GetInstantiatedMethod(this.GetMethodDefinition(), new Instantiation(clone));
        }

        public virtual MethodDesc GetMethodDefinition() => this;

        public override string ToString()
        {
            var sb = new StringBuilder();

            var typeNameFormatter = new TypeNameFormatter();

            typeNameFormatter.AppendName(sb, Signature.ReturnType);
            sb.Append(' ');

            sb.Append(OwningType);
            sb.Append("::");
            sb.Append(Name);

            var first = true;
            for (var i = 0; i < Instantiation.Length; i++)
            {
                if (first)
                {
                    sb.Append('<');
                    first = false;
                }
                else
                {
                    sb.Append(',');
                }
                first = false;
                typeNameFormatter.AppendName(sb, Instantiation[i]);
            }
            if (!first)
            {
                sb.Append('>');
            }

            sb.Append('(');
            sb.Append(Signature.ToString(includeReturnType: false));
            sb.Append(')');

            return sb.ToString();
        }

        public abstract IEnumerable<MethodImplRecord> Overrides { get; }
        public virtual string? GetCustomAttributeValue(string customAttributeName) => null;
        public abstract MethodDesc CreateUserMethod(string name);
    }
}