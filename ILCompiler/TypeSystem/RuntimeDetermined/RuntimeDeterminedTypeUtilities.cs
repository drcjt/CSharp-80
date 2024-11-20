using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.RuntimeDetermined
{
    public static class RuntimeDeterminedTypeUtilities
    {
        public static Instantiation ConvertInstantiationToSharedRuntimeForm(Instantiation instantiation, Instantiation openInstantiation, out bool changed)
        {
            TypeDesc[]? sharedInstantiation = null;
            var currentPolicy = CanonicalFormKind.Specific;

            for (int instantiationIndex = 0; instantiationIndex < instantiation.Length; instantiationIndex++)
            {
                var typeToConvert = instantiation[instantiationIndex];
                var context = typeToConvert.Context;
                var canonForm = context.ConvertToCanon(typeToConvert, ref currentPolicy);
                var runtimeDeterminedForm = typeToConvert;

                if ((typeToConvert != canonForm) || typeToConvert.IsCanonicalType)
                {
                    if (sharedInstantiation == null)
                    {
                        sharedInstantiation = new TypeDesc[instantiation.Length];
                        for (int i = 0; i < instantiationIndex; i++)
                            sharedInstantiation[i] = instantiation[i];
                    }

                    runtimeDeterminedForm = context.GetRuntimeDeterminedType((DefType)canonForm, (GenericParameterDesc)openInstantiation[instantiationIndex]);
                }

                if (sharedInstantiation != null)
                {
                    sharedInstantiation[instantiationIndex] = runtimeDeterminedForm;
                }
            }

            changed = sharedInstantiation != null;
            return sharedInstantiation != null ? new Instantiation(sharedInstantiation) : instantiation;
        }
    }
}
