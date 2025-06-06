﻿using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.IL.Stubs
{
    internal static class ComparerIntrinsics
    {
        public static MethodIL? EmitEqualityComparer(MethodDesc method)
        {
            return EmitComparerAndEqualityComparerCreateCommon(method, "EqualityComparer", "IEquatable`1");
        }

        private static MethodIL? EmitComparerAndEqualityComparerCreateCommon(MethodDesc methodBeingGenerated, string flavor, string interfaceName)
        {
            var owningType = methodBeingGenerated.OwningType;
            var comparedType = owningType.Instantiation![0];

            if (comparedType.IsCanonicalSubtype(CanonicalFormKind.Any))
                throw new NotImplementedException("No support for using type loader to load proper EqualityComparer");

            var comparerType = GetComparerForType(comparedType, flavor, interfaceName);

            var body = new MethodIL();

            var comparerTypeCtor = comparerType.GetKnownMethod(".ctor");

            body.Instructions.Add(new Instruction(ILOpcode.newobj, 0, comparerTypeCtor));
            body.Instructions.Add(new Instruction(ILOpcode.ret, 1));

            return body;
        }

        private static TypeDesc GetComparerForType(TypeDesc type, string flavor, string interfaceName)
        {
            var context = type.Context;

            if (context.IsCanonicalDefinitionType(type, CanonicalFormKind.Any) || (type.IsRuntimeDeterminedSubtype && !type.HasInstantiation))
            {
                // Can't determine the exact type for the comparer at compile time.
                throw new NotImplementedException("Runtime determined comparer types not supported");
            }

            bool? implementsInterfaceOfSelf = ImplementsInterfaceOfSelf(type, interfaceName);
            if (implementsInterfaceOfSelf is null)
            {
                throw new ArgumentException($"Type {type.FullName} does not implement {interfaceName}");
            }

            var comparerTypeName = (bool)implementsInterfaceOfSelf ? $"Generic{flavor}`1" : $"Object{flavor}`1";
            return context.SystemModule!.GetKnownType("System.Collections.Generic", comparerTypeName).MakeInstantiatedType(type);
        }


        public static bool? ImplementsIEquatable(TypeDesc type) => ImplementsInterfaceOfSelf(type, "IEquatable`1");

        private static bool? ImplementsInterfaceOfSelf(TypeDesc type, string interfaceName)
        {
            MetadataType? interfaceType = null;

            foreach (TypeDesc implementedInterface in type.RuntimeInterfaces)
            {
                Instantiation? interfaceInstantiation = implementedInterface.Instantiation;
                if (interfaceInstantiation?.Length == 1)
                {
                    if (interfaceType is null)
                    {
                        interfaceType = type.Context.SystemModule?.GetKnownType("System", interfaceName);
                    }

                    var result = ImplementsGenericInterfaceWithInstantiation(type, interfaceType, implementedInterface, interfaceInstantiation);
                    if (result is not null)
                        return result;
                }
            }

            return false;
        }

        private static bool? ImplementsGenericInterfaceWithInstantiation(TypeDesc type, MetadataType? interfaceType, TypeDesc implementedInterface, Instantiation interfaceInstantiation)
        {
            if (implementedInterface.GetTypeDefinition() == interfaceType)
            {
                if (type.IsCanonicalSubtype(CanonicalFormKind.Any))
                {
                    // Ignore interface instantiations that cannot possibly be the interface of self
                    if (implementedInterface.ConvertToCanonForm(CanonicalFormKind.Specific) !=
                        interfaceType.MakeInstantiatedType(type).ConvertToCanonForm(CanonicalFormKind.Specific))
                    {
                        return null;
                    }
                    // Try to prove that the interface of self is always implemented using the type definition.
                    TypeDesc typeDefinition = type.GetTypeDefinition();
                    return typeDefinition.CanCastTo(interfaceType.MakeInstantiatedType(typeDefinition)) ? true : null;
                }
                else
                {
                    // If the instantiation is exactly the type then we are done
                    if (interfaceInstantiation[0] == type)
                    {
                        Debug.Assert(type.CanCastTo(interfaceType.MakeInstantiatedType(type)));
                        return true;
                    }
                    return type.CanCastTo(interfaceType.MakeInstantiatedType(type));
                }
            }
            return null;
        }
    }
}
