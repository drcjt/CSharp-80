using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;
using System.Diagnostics;

namespace ILCompiler.IL.Stubs
{
    internal static class ComparerIntrinsics
    {
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
