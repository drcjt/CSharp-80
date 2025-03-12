using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.IL;
using ILCompiler.TypeSystem.Interop;
using ILCompiler.TypeSystem.RuntimeDetermined;
using System.Text;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class MethodDesc : TypeSystemEntity
    {
        public bool LocallocUsed { get; set; } = false;

        public virtual bool IsIntrinsic => false;

        public virtual bool IsPInvoke => false;

        public virtual PInvokeMetaData? GetPInvokeMetaData() => default;

        public virtual bool IsInternalCall => false;

        public virtual bool IsStaticConstructor => OwningType.GetStaticConstructor() == this;

        public virtual bool IsDefaultConstructor => OwningType.GetDefaultConstructor() == this;

        public virtual bool IsStatic => false;

        public abstract IList<MethodParameter> Parameters { get; }

        public abstract IList<LocalVariableDefinition> Locals { get; }

        public virtual bool HasReturnType => false;

        public virtual bool HasThis => false;
        public virtual bool IsExplicitThis => false;

        public bool IsConstructor => Name == ".ctor";

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

        public bool HasInstantiation => Instantiation.Length != 0;

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
                return Context.GetMethodForInstantiatedType(this.GetTypicalMethodDefinition(), (InstantiatedType)instantiatedOwningType);
            }
            
            return Context.GetInstantiatedMethod(this.GetMethodDefinition(), new Instantiation(clone));
        }

        public virtual MethodDesc GetMethodDefinition() => this;

        public virtual MethodDesc GetTypicalMethodDefinition() => this;

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

        public virtual MethodDesc GetCanonMethodTarget(CanonicalFormKind kind) => this;

        public virtual bool IsCanonicalMethod(CanonicalFormKind policy) => false;
        public bool IsSharedByGenericInstantiations => IsCanonicalMethod(CanonicalFormKind.Any);

        public TypeDesc ImplementationType => OwningType;

        public bool RequiresInstArg()
        {
            return IsSharedByGenericInstantiations &&
                   (HasInstantiation ||
                    Signature.IsStatic ||
                    ImplementationType.IsValueType ||
                    (ImplementationType.IsInterface && IsAbstract));
        }

        public bool RequiresInstMethodDescArg => HasInstantiation && IsSharedByGenericInstantiations;

        public bool AcquiresInstMethodTableFromThis()
        {
            return IsSharedByGenericInstantiations &&
                !HasInstantiation &&
                !Signature.IsStatic &&
                !ImplementationType.IsValueType &&
                !(ImplementationType.IsInterface && !IsAbstract);
        }

        public MethodDesc GetSharedRuntimeFormMethodTarget()
        {
            MethodDesc result = this;
            if (OwningType is DefType owningType)
            {
                DefType sharedRuntimeOwningType = owningType.ConvertToSharedRuntimeDeterminedForm();
                if (sharedRuntimeOwningType != owningType)
                {
                    result = Context.GetMethodForInstantiatedType(GetTypicalMethodDefinition(), (InstantiatedType)sharedRuntimeOwningType);
                }

                if (result.HasInstantiation)
                {
                    MethodDesc uninstantiatedMethod = result.GetMethodDefinition();

                    Instantiation sharedInstantiation = RuntimeDeterminedTypeUtilities.ConvertInstantiationToSharedRuntimeForm(
                        Instantiation, uninstantiatedMethod.Instantiation, out bool changed);

                    if (changed || result != this)
                    {
                        // Find matching instantiated method if the instantiation changed or the owning type was switched
                        result = Context.GetInstantiatedMethod(uninstantiatedMethod, sharedInstantiation);
                    }
                }
            }

            return result;
        }
    }
}