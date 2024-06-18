﻿namespace ILCompiler.TypeSystem.Common
{
    public class TypeSystemContext
    {        
        public TargetDetails Target { get; } = new TargetDetails(TargetArchitecture.Z80);

        private readonly Dictionary<string, ArrayType> _arrayTypesByFullName = new Dictionary<string, ArrayType>();
        private readonly Dictionary<string, FunctionPointerType> _functionPointerTypesBySignature = new Dictionary<string, FunctionPointerType>();
        private readonly Dictionary<string, PointerType> _pointerTypesByFullName = new Dictionary<string, PointerType>();
        private readonly Dictionary<string, FieldDesc> _fieldForInstantiatedTypesByFullName = new Dictionary<string, FieldDesc>();
        private readonly Dictionary<string, MethodDesc> _methodForInstantiatedTypesByFullName = new Dictionary<string, MethodDesc>();
        public ModuleDesc? SystemModule { get; set; }

        public InstantiatedType GetInstantiatedType(MetadataType typeDef, Instantiation instantiation)
        {
            return new InstantiatedType(typeDef, instantiation);
        }

        public InstantiatedMethod GetInstantiatedMethod(MethodDesc methodDef, Instantiation instantiation)
        {
            if (methodDef is InstantiatedMethod instantiated)
            {
                var currentInstantiation = instantiated.Instantiation;

                TypeDesc[] genericParameters = new TypeDesc[currentInstantiation.Length];
                for (var i = 0; i < currentInstantiation.Length; i++)
                {
                    var typeDesc = currentInstantiation[i];
                    genericParameters[i] = typeDesc.InstantiateSignature(default(Instantiation), instantiation);
                }
                var newInstantiation = new Instantiation(genericParameters);

                return new InstantiatedMethod(instantiated.GetMethodDefinition(), newInstantiation);
            }

            return new InstantiatedMethod(methodDef, instantiation);
        }

        public IEnumerable<MethodDesc> GetAllVirtualMethods(TypeDesc type) => type.GetVirtualMethods();

        public DefType GetClosestDefType(TypeDesc type)
        {
            if (type.IsArray)
            {
                return (DefType)SystemModule!.GetType("System", "Array");
            }

            return ((DefType)type);
        }

        public ArrayType GetArrayType(TypeDesc elementType, int rank)
        {
            var arrayTypeKey = $"{elementType.FullName}_{rank}";
            if (!_arrayTypesByFullName.ContainsKey(arrayTypeKey))
            {
                _arrayTypesByFullName[arrayTypeKey] = new ArrayType(elementType, rank);
            }

            return _arrayTypesByFullName[arrayTypeKey];
        }

        public FunctionPointerType GetFunctionPointerType(MethodSignature signature)
        {
            if (!_functionPointerTypesBySignature.ContainsKey(signature.ToString()))
            {
                _functionPointerTypesBySignature[signature.ToString()] = new FunctionPointerType(signature);
            }
            return _functionPointerTypesBySignature[signature.ToString()];
        }

        public PointerType GetPointerType(TypeDesc parameterType)
        {
            if (!_pointerTypesByFullName.ContainsKey(parameterType.FullName))
            {
                _pointerTypesByFullName[parameterType.FullName] = new PointerType(parameterType);
            }
            return _pointerTypesByFullName[parameterType.FullName];
        }

        public FieldDesc GetFieldForInstantiatedType(FieldDesc fieldDef, InstantiatedType instantiatedType)
        {
            // TODO: Fix key generation as fullname/instantiation may contain colons
            var fieldForInstantiatedTypeKey = fieldDef.FullName + ":" + instantiatedType.Instantiation!.ToString();
            if (!_fieldForInstantiatedTypesByFullName.ContainsKey(fieldForInstantiatedTypeKey))
            {
                _fieldForInstantiatedTypesByFullName[fieldForInstantiatedTypeKey] = new FieldForInstantiatedType(fieldDef, instantiatedType);
            }
            return _fieldForInstantiatedTypesByFullName[fieldForInstantiatedTypeKey];
        }

        public MethodDesc GetMethodForInstantiatedType(MethodDesc typicalMethodDef, InstantiatedType instantiatedType)
        {
            var methodForInstantiatedTypeKey = typicalMethodDef.FullName + ":" + instantiatedType.Instantiation!.ToString();
            if (!_methodForInstantiatedTypesByFullName.ContainsKey(methodForInstantiatedTypeKey))
            {
                _methodForInstantiatedTypesByFullName[methodForInstantiatedTypeKey] = new MethodForInstantiatedType(typicalMethodDef, instantiatedType);
            }
            return _methodForInstantiatedTypesByFullName[methodForInstantiatedTypeKey];
        }
    }
}