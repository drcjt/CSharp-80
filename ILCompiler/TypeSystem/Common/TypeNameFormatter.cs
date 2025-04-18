﻿using System.Diagnostics;
using System.Text;

namespace ILCompiler.TypeSystem.Common
{
    internal class TypeNameFormatter
    {
        public void AppendName(StringBuilder sb, TypeDesc type)
        {
            if (type is ArrayType arrayType)
                AppendName(sb, arrayType);
            else if (type is PointerType pointerType)
                AppendName(sb, pointerType);
            else if (type is FunctionPointerType functionPointerType)
                AppendName(sb, functionPointerType);
            else if (type is GenericParameterDesc genericParameterDesc)
                AppendName(sb, genericParameterDesc);
            else if (type is SignatureTypeVariable signatureTypeVariable)
                AppendName(sb, signatureTypeVariable);
            else if (type is SignatureMethodVariable signatureMethod)
                AppendName(sb, signatureMethod);
            else if (type is ByRefType byrefType)
                AppendName(sb, byrefType);
            else if (type is InstantiatedType instantiatedType)
                AppendName(sb, instantiatedType);
            else
            {
                Debug.Assert(type is DefType);
                AppendName(sb, (DefType)type);
            }
        }

        public void AppendName(StringBuilder sb, InstantiatedType instantiatedType)
        {
            AppendName(sb, instantiatedType.GetTypeDefinition());
            if (instantiatedType.Instantiation != null)
            {
                sb.Append('<');
                for (int i = 0; i < instantiatedType.Instantiation.Length; i++)
                {
                    if (i != 0)
                        sb.Append(',');
                    AppendName(sb, instantiatedType.Instantiation[i]);
                }
                sb.Append('>');
            }
        }

        public void AppendName(StringBuilder sb, ByRefType type)
        {
            AppendName(sb, type.ParameterType);
            sb.Append('&');
        }

        public static void AppendName(StringBuilder sb, SignatureMethodVariable signatureMethod)
        {
            sb.Append("!!");
            sb.Append(signatureMethod.Index);
        }

        public static void AppendName(StringBuilder sb, SignatureTypeVariable signatureTypeVariable)
        {
            sb.Append('!');
            sb.Append(signatureTypeVariable.Index);
        }

        public static void AppendName(StringBuilder sb, GenericParameterDesc genericParameterDesc)
        {
            sb.Append(genericParameterDesc.Kind == GenericParameterKind.Type ? "!" : "!!");
            sb.Append(genericParameterDesc.Name);
        }

        public void AppendName(StringBuilder sb, FunctionPointerType functionPointerType)
        {
            throw new NotImplementedException();
        }

        public void AppendName(StringBuilder sb, PointerType pointerType)
        {
            AppendName(sb, pointerType.ParameterType);
            sb.Append('*');
        }

        public void AppendName(StringBuilder sb, ArrayType arrayType)
        {
            AppendName(sb, arrayType.ElementType);
            sb.Append('[');
            sb.Append(',', arrayType.Rank - 1);
            sb.Append(']');
        }

        public void AppendName(StringBuilder sb, DefType type)
        {
            if (type.IsInstantiatedType)
            {
                AppendNameForInstantiatedType(sb, type);
            }
            else
            {
                DefType? containingType = type.ContainingType;
                if (containingType is not null)
                {
                    AppendNameForNestedType(sb, type, containingType);
                }
                else
                {
                    AppendNameForNamespaceType(sb, type);
                }
            }
        }

        protected void AppendNameForNestedType(StringBuilder sb, DefType nestedType, DefType containingType)
        {
            AppendName(sb, containingType);
            sb.Append('+');
            sb.Append(nestedType.Name);
        }


        private void AppendNameForInstantiatedType(StringBuilder sb, DefType type)
        {
            AppendName(sb, type.GetTypeDefinition());
            sb.Append('<');

            for (int i = 0; i < type.Instantiation!.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                AppendName(sb, type.Instantiation[i]);
            }

            sb.Append('>');
        }

        public static void AppendNameForNamespaceType(StringBuilder sb, DefType type)
        {
            string ns = type.Namespace;
            if (ns.Length > 0)
            {
                sb.Append(ns);
                sb.Append('.');
            }
            sb.Append(type.Name);
        }
    }
}
