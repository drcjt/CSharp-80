using ILCompiler.Compiler;
using System.Text;

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


        public new bool IsSzArray => _rank < 0;
        public bool IsMdArray => _rank > 0;

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
    }
}
