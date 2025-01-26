using System.Diagnostics;

namespace ILCompiler.TypeSystem.Common
{
    public static class CastingHelper
    {
        /// <summary>
        /// Returns trus if '<paramref name="thisType"/>' can be cast to '<paramref name="otherType"/>'.
        /// If '<paramref name="thisType"/>' is a value type assume it is boxed, so
        /// CanCastTo of System.Int32 to System.Object will return true
        /// </summary>
        /// <param name="thisType"></param>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public static bool CanCastTo(this TypeDesc thisType, TypeDesc otherType)
        {
            return thisType.CanCastToInternal(otherType);
        }
        private static bool CanCastToInternal(this TypeDesc thisType, TypeDesc otherType)
        {
            if (thisType == otherType)
            {
                return true;
            }
            
            // TODO: Handle GenericParameters, Arrays, SzArrays, ByRefs, Pointers, FunctionPointers

            Debug.Assert(thisType.IsDefType);
            return thisType.CanCastToClassOrInterface(otherType);
        }

        private static bool CanCastToClassOrInterface(this TypeDesc thisType, TypeDesc otherType)
        {
            if (otherType.IsInterface)
            {
                return thisType.CanCastToInterface(otherType);
            }
            else
            {
                return thisType.CanCastToClass(otherType);
            }
        }

        private static bool CanCastToInterface(this TypeDesc thisType, TypeDesc otherType)
        {
            // TODO: Handle variance properly

            if (thisType.CanCastByVarianceToInterfaceOrDelegate(otherType))
            {
                return true;
            }

            foreach (var interfaceType in thisType.RuntimeInterfaces)
            {
                if (interfaceType.CanCastByVarianceToInterfaceOrDelegate(otherType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CanCastByVarianceToInterfaceOrDelegate(this TypeDesc thisType, TypeDesc otherType)
        {
            if (thisType == otherType)
            {
                return true;
            }

            // TODO: Handle non trivial case

            return false;
        }

        private static bool CanCastToClass(this TypeDesc thisType, TypeDesc otherType)
        {
            TypeDesc? currentType = thisType;

            if (currentType.IsInterface && otherType.IsObject)
            {
                return true;
            }

            // TODO: support variance properly
            do
            {
                if (currentType.IsEquivalentTo(otherType))
                    return true;

                currentType = currentType.BaseType;
            } while (currentType != null);

            return false;
        }


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