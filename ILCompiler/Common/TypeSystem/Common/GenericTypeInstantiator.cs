using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common
{
    internal static class GenericTypeInstantiator
    {
        public static TypeSig Instantiate(TypeSig type, IMethod calleeMethod, MethodDesc callerMethod) 
        {
            if (type.ContainsGenericParameter) 
            {
                IList<TypeSig> callerMethodGenericParameters = new List<TypeSig>();
                if (callerMethod is InstantiatedMethod method)
                {
                    callerMethodGenericParameters = method.GenericParameters;
                }

                var resolvedGenericParameters = new List<TypeSig>();
                foreach (var genericParameter in ((MethodSpec)calleeMethod).GenericInstMethodSig.GenericArguments)
                {
                    resolvedGenericParameters.Add(GenericTypeInstantiator.Instantiate(genericParameter, callerMethodGenericParameters));
                }

                return Instantiate(type, resolvedGenericParameters);
            }
            else
            {
                return type;
            }
        }

        public static TypeSig Instantiate(TypeSig type, IList<TypeSig> genericMethodParameters)
        {
            TypeSig result = type;
            switch (type.ElementType)
            {
                case ElementType.Ptr:
                    result = new PtrSig(Instantiate(type.Next, genericMethodParameters));
                    break;

                case ElementType.ByRef:
                    result = new ByRefSig(Instantiate(type.Next, genericMethodParameters));
                    break;

                case ElementType.Array:
                    var arraySig = (ArraySig)type;
                    result = new ArraySig(arraySig.Next, arraySig.Rank, arraySig.Sizes, arraySig.LowerBounds);
                    break;

                case ElementType.SZArray:
                    result = new SZArraySig(Instantiate(type.Next, genericMethodParameters));
                    break;

                case ElementType.Pinned:
                    result = new PinnedSig(Instantiate(type.Next, genericMethodParameters));
                    break;

                case ElementType.Var:
                    throw new NotImplementedException("Generic arguments not yet implemented");

                case ElementType.MVar:
                    if (type is GenericSig varSig && varSig.Number < genericMethodParameters.Count)
                    {
                        result = genericMethodParameters[(int)varSig.Number];
                    }
                    break;

                default:
                    break;
            }

            return result;
        }
    }
}
