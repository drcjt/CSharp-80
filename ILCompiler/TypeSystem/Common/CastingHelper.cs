namespace ILCompiler.TypeSystem.Common
{
    public static class CastingHelper
    {
        public static bool IsEquivalentTo(this TypeDesc thisType, TypeDesc otherType)
        {
            if (thisType == otherType)
                return true;

            if (thisType is SignatureTypeVariable || thisType is SignatureMethodVariable)
                return false;

            if (thisType is ArrayType)
            {
                var arrayType = (ArrayType)thisType;
                var otherArrayType = (ArrayType)otherType;
                if (arrayType.Rank != otherArrayType.Rank)
                    return false;

                return arrayType.ParameterType.IsEquivalentTo(otherArrayType.ParameterType);
            }

            if (thisType is ByRefType || thisType is PointerType)
            {
                return ((ParameterizedType)thisType).ParameterType.IsEquivalentTo(((ParameterizedType)otherType).ParameterType);
            }

            return ((DefType)thisType).IsEquivalentToDefType((DefType)otherType);
        }

        private static bool IsEquivalentToDefType(this DefType thisType, DefType otherType)
        {
            if (thisType.Name != otherType.Name)
                return false;

            if (thisType.Namespace != otherType.Namespace)
                return false;

            if (thisType.IsInterface != otherType.IsInterface)
                return false;

            if (thisType.IsInterface)
                return true;

            if (thisType.IsEnum != otherType.IsEnum)
                return false;

            if (thisType.IsValueType != otherType.IsValueType)
                return false;

            if (thisType.IsEnum)
                return CompareStructuresForEquivalence(thisType, otherType, enumMode: true);

            if (thisType.IsValueType)
                return CompareStructuresForEquivalence(thisType, otherType, enumMode: false);

            return true;
        }

        private static bool CompareStructuresForEquivalence(DefType type1, DefType type2, bool enumMode)
        {
            if (type1.GetMethods().Any())
            {
                return false;
            }

            if (type2.GetMethods().Any())
            {
                return false;
            }

            var fields1 = type1.GetFields().GetEnumerator();
            var fields2 = type2.GetFields().GetEnumerator();

            while (true)
            {
                bool nonTypeEquivalentValidFieldFound;

                var field1 = GetNextTypeEquivalentField(fields1, enumMode, out nonTypeEquivalentValidFieldFound);
                if (nonTypeEquivalentValidFieldFound)
                    return false;
                var field2 = GetNextTypeEquivalentField(fields2, enumMode, out nonTypeEquivalentValidFieldFound);
                if (nonTypeEquivalentValidFieldFound)
                    return false;

                if (field1 == null && field2 == null)
                    break;

                if (field1 == null || field2 == null)
                    break;

                if (!field1.FieldType.IsEquivalentTo(field2.FieldType))
                    return false;
            }

            if (!enumMode)
            {
                // TODO: Compare Type Layouts
            }

            return true;
        }

        static FieldDesc? GetNextTypeEquivalentField(IEnumerator<FieldDesc> fieldEnum, bool enumMode, out bool fieldNotValidInEquivalentTypeFound)
        {
            fieldNotValidInEquivalentTypeFound = false;
            while (fieldEnum.MoveNext())
            {
                var field = fieldEnum.Current;

                if (field.EffectiveVisibility == EffectiveVisibility.Public && !field.IsStatic)
                    return field;

                // Only public instance fields, and literal fields on enums are permitted in type equivalent structures
                if (!enumMode || !field.IsLiteral)
                {
                    fieldNotValidInEquivalentTypeFound = true;
                    return null;
                }
            }
            return null;
        }
    }
}