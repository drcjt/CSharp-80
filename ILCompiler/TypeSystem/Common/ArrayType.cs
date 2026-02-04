using ILCompiler.Compiler;
using ILCompiler.IL.Stubs;
using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Common
{
    public class ArrayType : ParameterizedType
    {
        private readonly int _rank;

        public ArrayType(TypeDesc elementType, int rank) : base(elementType)
        {
            _rank = rank;
        }

        public TypeDesc ElementType => this.ParameterType;

        public override DefType BaseType => Context.GetWellKnownType(WellKnownType.Array);

        public override bool IsArray => true;
        public override bool IsSzArray => _rank < 0;
        public override bool IsMdArray => _rank > 0;

        public int Rank => _rank < 0 ? 1 : _rank;

        public override VarType VarType => VarType.Ref;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var instantiatedElementType = ElementType.InstantiateSignature(typeInstantiation, methodInstantiation);
            if (instantiatedElementType != ElementType)
            {
                return Context.GetArrayType(instantiatedElementType, _rank);
            }

            return this;
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            TypeDesc paramTypeConverted = Context.ConvertToCanon(ParameterType, kind);
            if (paramTypeConverted != ParameterType)
            {
                return Context.GetArrayType(paramTypeConverted, _rank);
            }

            return this;
        }

        protected override TypeFlags ComputeTypeFlags() => _rank == -1 ? TypeFlags.SzArray : TypeFlags.Array;

        private MethodDesc[]? _methods;
        public override IEnumerable<MethodDesc> GetMethods()
        {
            if (_methods == null)
            {
                InitializeMethods();
            }
            return _methods!;
        }

        private void InitializeMethods()
        {
            int numberOfConstructors;
            if (IsSzArray)
            {
                numberOfConstructors = 1;

                // Jagged arrays have constructors for each depth
                var elemType = ElementType;
                while (elemType.IsSzArray)
                {
                    elemType = ((ArrayType)elemType).ElementType;
                    numberOfConstructors++;
                }
            }
            else
            {
                // Multidimensional arrays have two constructors
                // One with and one without the lower bounds
                numberOfConstructors = 2;
            }

            MethodDesc[] methods = new MethodDesc[(int)ArrayMethodKind.Ctor + numberOfConstructors];
            for (int i = 0; i < methods.Length; i++)
            {
                methods[i] = new ArrayMethod(this, (ArrayMethodKind)i);
            }

            _methods = methods;
        }

        public MethodDesc GetArrayMethod(ArrayMethodKind kind)
        {
            if (_methods == null)
            {
                InitializeMethods();
            }
            return _methods![(int)kind];
        }
    }

    public enum ArrayMethodKind
    {
        Get,
        Set,
        Address,
        AddressWithHiddenArg,
        Ctor
    }

    public class ArrayMethod : MethodDesc
    {
        private readonly ArrayType _owningType;
        public ArrayMethodKind Kind { get; }

        public ArrayMethod(ArrayType owningType, ArrayMethodKind kind)
        {
            _owningType = owningType;
            Kind = kind;
        }

        public override TypeSystemContext Context => _owningType.Context;

        public override TypeDesc OwningType => _owningType;

        public override bool HasReturnType 
            => Kind switch
            {
                ArrayMethodKind.Get or ArrayMethodKind.Address or ArrayMethodKind.AddressWithHiddenArg => true,
                _ => false,
            };

        public override bool HasThis
            => Kind switch
            {
                ArrayMethodKind.Ctor => false,
                _ => true,
            };

        public override MethodSignature Signature
            => Kind switch
            {
                ArrayMethodKind.Get => CreateGetMethodSignature(),
                ArrayMethodKind.Set => CreateSetMethodSignature(),
                ArrayMethodKind.Address => CreateAddressMethodSignature(),
                ArrayMethodKind.AddressWithHiddenArg => CreateAddressWithHiddenArgMethodSignature(),
                _ => CreateCtorMethodSignature(),
            };

        private MethodSignature CreateCtorMethodSignature()
        {
            int numberOfParameters;
            if (_owningType.IsSzArray)
            {
                numberOfParameters = 1 + (int)Kind - (int)ArrayMethodKind.Ctor;
            }
            else
            {
                numberOfParameters = (Kind == ArrayMethodKind.Ctor) ? _owningType.Rank : 2 * _owningType.Rank;
            }
            var parameters = new MethodParameter[numberOfParameters];
            for (int i = 0; i < numberOfParameters; i++)
            {
                parameters[i] = new MethodParameter(Context.GetWellKnownType(WellKnownType.Int32), String.Empty);
            }

            return new MethodSignature(MethodSignatureFlags.None, Context.GetWellKnownType(WellKnownType.Void), parameters);
        }

        private MethodSignature CreateAddressWithHiddenArgMethodSignature()
        {
            var parameters = new MethodParameter[_owningType.Rank + 1];
            parameters[0] = new MethodParameter(_owningType.Context.GetPointerType(_owningType.Context.GetWellKnownType(WellKnownType.Void)), String.Empty);
            for (int i = 0; i < _owningType.Rank; i++)
                parameters[i + 1] = new MethodParameter(_owningType.Context.GetWellKnownType(WellKnownType.Int32), String.Empty);
            return new MethodSignature(MethodSignatureFlags.None, _owningType.ElementType.MakeByRefType(), parameters);
        }

        private MethodSignature CreateAddressMethodSignature()
        {
            var parameters = new MethodParameter[_owningType.Rank];
            for (int i = 0; i < _owningType.Rank; i++)
                parameters[i] = new MethodParameter(_owningType.Context.GetWellKnownType(WellKnownType.Int32), String.Empty);
            return new MethodSignature(MethodSignatureFlags.None, _owningType.ElementType.MakeByRefType(), parameters);
        }

        private MethodSignature CreateSetMethodSignature()
        {
            var parameters = new MethodParameter[_owningType.Rank + 1];
            for (int i = 0; i < _owningType.Rank; i++)
                parameters[i] = new MethodParameter(_owningType.Context.GetWellKnownType(WellKnownType.Int32), String.Empty);
            parameters[_owningType.Rank] = new MethodParameter(_owningType.ElementType, String.Empty);
            return new MethodSignature(MethodSignatureFlags.None, this.Context.GetWellKnownType(WellKnownType.Void), parameters);
        }

        private MethodSignature CreateGetMethodSignature()
        {
            var parameters = new MethodParameter[_owningType.Rank];
            for (int i = 0; i < _owningType.Rank; i++)
                parameters[i] = new MethodParameter(_owningType.Context.GetWellKnownType(WellKnownType.Int32), String.Empty);
            return new MethodSignature(MethodSignatureFlags.None, _owningType.ElementType, parameters);
        }

        public override string Name
            => Kind switch
            {
                ArrayMethodKind.Get => "Get",
                ArrayMethodKind.Set => "Set",
                ArrayMethodKind.Address or ArrayMethodKind.AddressWithHiddenArg => "Address",
                _ => ".ctor",
            };

        public override string FullName => ToString();

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName) => false;

        public override MethodDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var owningType = OwningType;
            var instantiatedOwningType = owningType.InstantiateSignature(typeInstantiation, methodInstantiation);

            if (owningType != instantiatedOwningType)
            {
                return ((ArrayType)instantiatedOwningType).GetArrayMethod(Kind);
            }

            return this;
        }

        public override IEnumerable<MethodImplRecord> Overrides => throw new NotImplementedException();

        public override Instantiation Instantiation => Instantiation.Empty;

        public override IList<MethodParameter> Parameters => Signature.Parameters;

        public override IList<LocalVariableDefinition> Locals => _methodIL?.Locals ?? [];

        private MethodIL? _methodIL;
        public override MethodIL? MethodIL
        {
            get
            {
                if (_methodIL == null)
                {
                    _methodIL = ArrayMethodILEmitter.EmitIL(this);
                }
                return _methodIL;
            }
        }

        public override MethodDesc CreateUserMethod(string name) => throw new NotImplementedException();
    }
}
