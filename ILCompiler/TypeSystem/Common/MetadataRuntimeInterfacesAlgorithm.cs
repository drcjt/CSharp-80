namespace ILCompiler.TypeSystem.Common
{
    public static class MetadataRuntimeInterfacesAlgorithm
    {
        /// <summary>
        /// Computes the complete list of interfaces the type implements
        /// </summary>
        /// <param name="typeDefOrRef"></param>
        /// <returns>List of interfaces </returns>
        /// <exception cref="NotImplementedException"></exception>
        public static DefType[] ComputeRuntimeInterfaces(TypeDesc typeDesc)
        {
            MetadataType type = (MetadataType)typeDesc;
            if (type.IsInstantiatedType)
            {
                return ComputeRuntimeInterfacesForInstantiatedType((InstantiatedType)type);
            }
            else
            {
                return ComputeRuntimeInterfacesForNonInstantiatedMetadataType(type);
            }

        }

        private static DefType[] ComputeRuntimeInterfacesForInstantiatedType(InstantiatedType instantiatedType)
        {
            MetadataType uninstantiatedType = (MetadataType)instantiatedType.GetTypeDefinition();

            return InstantiatedType.InstantiateTypeArray(uninstantiatedType.RuntimeInterfaces, instantiatedType.Instantiation, default(Instantiation));
        }

        /// <summary>
        /// Compute the interfaces for a non instantiated/non generic type
        /// </summary>
        /// <param name="typeDefOrRef"></param>
        /// <returns></returns>
        private static DefType[] ComputeRuntimeInterfacesForNonInstantiatedMetadataType(MetadataType type) 
        {
            DefType[] explicitInterfaces = type.ExplicitlyImplementedInterfaces;
            DefType[] baseTypeInterfaces = type.HasBaseType ? type.BaseType!.RuntimeInterfaces : Array.Empty<DefType>();

            var interfacesList = new List<DefType>(baseTypeInterfaces);
            foreach (var interfaceType in explicitInterfaces)
            {
                BuildPostOrderInterfaceList(interfaceType, interfacesList);
            }
            return interfacesList.ToArray();
        }

        /// <summary>
        /// Determine the required interfaces of an interface type and add these to the interfaces list
        /// Then finally add the interface type itself if it isn't already in the interfaces list
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="interfacesList"></param>
        private static void BuildPostOrderInterfaceList(DefType interfaceType, List<DefType> interfacesList)
        {
            if (interfacesList.Contains(interfaceType))
                return;

            foreach (DefType implementedInterface in interfaceType.RuntimeInterfaces)
            {
                BuildPostOrderInterfaceList(implementedInterface, interfacesList);
            }

            if (interfacesList.Contains(interfaceType))
                return;

            interfacesList.Add(interfaceType);
        }
    }
}
