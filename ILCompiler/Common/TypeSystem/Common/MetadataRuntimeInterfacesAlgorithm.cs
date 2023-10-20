using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Common.TypeSystem.Common
{
    public static class MetadataRuntimeInterfacesAlgorithm
    {
        /// <summary>
        /// Computes the complete list of interfaces the type implements
        /// </summary>
        /// <param name="typeDefOrRef"></param>
        /// <returns>List of interfaces </returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ITypeDefOrRef[] ComputeRuntimeInterfaces(ITypeDefOrRef typeDefOrRef)
        {
            if (!typeDefOrRef.ContainsGenericParameter)
            {
                return ComputeRuntimeInterfacesForNonInstantiatedMetadataType(typeDefOrRef);
            }

            throw new NotImplementedException("Generic interfaces not supported");
        }

        /// <summary>
        /// Compute the interfaces for a non instantiated/non generic type
        /// </summary>
        /// <param name="typeDefOrRef"></param>
        /// <returns></returns>
        private static ITypeDefOrRef[] ComputeRuntimeInterfacesForNonInstantiatedMetadataType(ITypeDefOrRef typeDefOrRef) 
        {
            var type = typeDefOrRef.ResolveTypeDefThrow();
            var explicitInterfaces = type.Interfaces.Select(x => x.Interface).ToList();
            var baseTypeInterfaces = type.BaseType?.RuntimeInterfaces() ?? Array.Empty<ITypeDefOrRef>();

            var interfacesList = new List<ITypeDefOrRef>(baseTypeInterfaces);
            foreach (ITypeDefOrRef interfaceType in explicitInterfaces)
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
        private static void BuildPostOrderInterfaceList(ITypeDefOrRef interfaceType, List<ITypeDefOrRef> interfacesList)
        {
            if (interfacesList.Contains(interfaceType))
                return;

            foreach (ITypeDefOrRef implementedInterface in interfaceType.RuntimeInterfaces())
            {
                BuildPostOrderInterfaceList(implementedInterface, interfacesList);
            }

            if (interfacesList.Contains(interfaceType))
                return;

            interfacesList.Add(interfaceType);
        }
    }
}
