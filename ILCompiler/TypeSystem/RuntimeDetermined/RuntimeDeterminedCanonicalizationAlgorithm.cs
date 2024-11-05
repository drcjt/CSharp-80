using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.RuntimeDetermined
{
    public static class RuntimeDeterminedCanonicalizationAlgorithm
    {
        public static Instantiation ConvertInstantiationToCanonForm(Instantiation instantiation, CanonicalFormKind kind, out bool changed)
        {
            TypeDesc[]? canonInstantiation = null;

            for (int instantiationIndex = 0; instantiationIndex < instantiation.Length; instantiationIndex++)
            {
                TypeDesc typeToConvert = instantiation[instantiationIndex];
                TypeDesc canonForm = ConvertToCanon(typeToConvert, kind);
                if (typeToConvert != canonForm || canonInstantiation != null)
                {
                    if (canonInstantiation == null)
                    {
                        canonInstantiation = new TypeDesc[instantiation.Length];
                        for (int i = 0; i < instantiationIndex; i++)
                            canonInstantiation[i] = instantiation[i];
                    }

                    canonInstantiation[instantiationIndex] = canonForm;
                }
            }

            changed = canonInstantiation != null;
            if (canonInstantiation != null)
            {
                return new Instantiation(canonInstantiation);
            }

            return instantiation;
        }

        public static TypeDesc ConvertToCanon(TypeDesc typeToConvert, CanonicalFormKind kind)
        {
            return ConvertToCanon(typeToConvert, ref kind);
        }

        public static TypeDesc ConvertToCanon(TypeDesc typeToConvert, ref CanonicalFormKind kind)
        {
            TypeSystemContext context = typeToConvert.Context;

            if (kind == CanonicalFormKind.Specific)
            {
                if (typeToConvert.IsSignatureVariable)
                {
                    return typeToConvert;
                }
                else if (typeToConvert.IsDefType)
                {
                    if (!typeToConvert.IsValueType)
                    {
                        // Reference types are treated canonically
                        return context.CanonType;
                    }
                    else
                    {
                        // Value types are not treated canonically
                        return typeToConvert;
                    }
                }
                else if (typeToConvert.IsArray)
                {
                    // Arrays are treated canonically
                    return context.CanonType;
                }
                else
                {
                    return typeToConvert.ConvertToCanonForm(CanonicalFormKind.Specific);
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
