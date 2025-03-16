using ILCompiler.Compiler;
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

        public override TypeFlags Category => _rank == -1 ? TypeFlags.SzArray : TypeFlags.Array;

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
        private readonly ArrayMethodKind _kind;

        public ArrayMethod(ArrayType owningType, ArrayMethodKind kind)
        {
            _owningType = owningType;
            _kind = kind;
        }

        public override TypeSystemContext Context => _owningType.Context;

        public override TypeDesc OwningType => _owningType;

        public override MethodSignature Signature
        {
            get
            {
                switch (_kind)
                {
                    case ArrayMethodKind.Get:
                    case ArrayMethodKind.Set:
                    case ArrayMethodKind.Address:
                    case ArrayMethodKind.AddressWithHiddenArg:
                        throw new NotImplementedException();

                    default:
                        {
                            int numberOfParameters;
                            if (_owningType.IsSzArray)
                            {
                                numberOfParameters = 1 + (int)_kind - (int)ArrayMethodKind.Ctor;
                            }
                            else
                            {
                                numberOfParameters = (_kind == ArrayMethodKind.Ctor) ? _owningType.Rank : 2 * _owningType.Rank;
                            }
                            var parameters = new MethodParameter[numberOfParameters];
                            for (int i = 0; i < numberOfParameters; i++)
                            {
                                parameters[i] = new MethodParameter(Context.GetWellKnownType(WellKnownType.Int32), String.Empty);
                            }

                            return new MethodSignature(MethodSignatureFlags.None, Context.GetWellKnownType(WellKnownType.Void), parameters);
                        }
                }
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                switch (_kind)
                {
                    case ArrayMethodKind.Get:
                        return "Get";
                    case ArrayMethodKind.Set:
                        return "Set";
                    case ArrayMethodKind.Address:
                    case ArrayMethodKind.AddressWithHiddenArg:
                        return "Address";
                    default:
                        return ".ctor";
                }
            }
        }

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName) => false;

        public override MethodDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var owningType = OwningType;
            var instantiatedOwningType = owningType.InstantiateSignature(typeInstantiation, methodInstantiation);

            if (owningType != instantiatedOwningType)
            {
                return ((ArrayType)instantiatedOwningType).GetArrayMethod(_kind);
            }

            return this;
        }

        public override IEnumerable<MethodImplRecord> Overrides => throw new NotImplementedException();

        public override Instantiation Instantiation => Instantiation.Empty;

        public override IList<MethodParameter> Parameters => Signature.Parameters;

        public override IList<LocalVariableDefinition> Locals => throw new NotImplementedException();

        public override MethodIL? MethodIL => throw new NotImplementedException();

        public override MethodDesc CreateUserMethod(string name)
        {
            throw new NotImplementedException();
        }
    }
}