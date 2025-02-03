using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Canon;

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
    }
}
