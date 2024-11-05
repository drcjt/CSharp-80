using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.RuntimeDetermined
{
    public class RuntimeDeterminedTypeUtilities
    {
        public static Instantiation ConvertInstantiationToSharedRuntimeForm(Instantiation instantiation, Instantiation openInstantiation, out bool changed)
        {
            TypeDesc[]? sharedInstantiation = null;

            CanonicalFormKind currentPolicy = CanonicalFormKind.Specific;
            CanonicalFormKind startLoopPolicy;

            do
            {
                startLoopPolicy = currentPolicy;

                for (int instantiationIndex = 0; instantiationIndex < instantiation.Length; instantiationIndex++)
                {
                    TypeDesc typeToConvert = instantiation[instantiationIndex];
                    TypeSystemContext context = typeToConvert.Context;
                    TypeDesc canonForm = context.ConvertToCanon(typeToConvert, ref currentPolicy);
                    TypeDesc runtimeDeterminedForm = typeToConvert;

                    if ((typeToConvert != canonForm) || typeToConvert.IsCanonicalType)
                    {
                        if (sharedInstantiation == null)
                        {
                            sharedInstantiation = new TypeDesc[instantiation.Length];
                            for (int i = 0; i < instantiationIndex; i++)
                                sharedInstantiation[i] = instantiation[i];
                        }

                        runtimeDeterminedForm = context.GetRuntimeDeterminedType(
                            (DefType)canonForm, (GenericParameterDesc)openInstantiation[instantiationIndex]);
                    }

                    if (sharedInstantiation != null)
                    {
                        sharedInstantiation[instantiationIndex] = runtimeDeterminedForm;
                    }
                }

                // Optimization: even if canonical policy changed, we don't actually need to re-run the loop
                // for instantiations that only have a single element.
                if (instantiation.Length == 1)
                {
                    break;
                }

            } while (currentPolicy != startLoopPolicy);

            changed = sharedInstantiation != null;
            if (sharedInstantiation != null)
            {
                return new Instantiation(sharedInstantiation);
            }

            return instantiation;
        }
    }
}
