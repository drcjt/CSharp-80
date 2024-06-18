namespace ILCompiler.TypeSystem.Common
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
            if (_arrayTypesByFullName.TryGetValue(arrayTypeKey, out var _array))
                return _array;

            return _arrayTypesByFullName[arrayTypeKey] = new ArrayType(elementType, rank);
        }

        public FunctionPointerType GetFunctionPointerType(MethodSignature signature)
        {
            if (_functionPointerTypesBySignature.TryGetValue(signature.ToString(), out var functionPointer))
                return functionPointer;

            return _functionPointerTypesBySignature[signature.ToString()] = new FunctionPointerType(signature);
        }

        public PointerType GetPointerType(TypeDesc parameterType)
        {
            if (_pointerTypesByFullName.TryGetValue(parameterType.FullName, out var pointer))
                return pointer;

            return _pointerTypesByFullName[parameterType.FullName] = new PointerType(parameterType);
        }

        public FieldDesc GetFieldForInstantiatedType(FieldDesc fieldDef, InstantiatedType instantiatedType)
        {
            // TODO: Fix key generation as fullname/instantiation may contain colons
            var key = fieldDef.FullName + ":" + instantiatedType.Instantiation!.ToString();
            if (_fieldForInstantiatedTypesByFullName.TryGetValue(key, out var field)) 
                return field; 

            return _fieldForInstantiatedTypesByFullName[key] = new FieldForInstantiatedType(fieldDef, instantiatedType);
        }

        public MethodDesc GetMethodForInstantiatedType(MethodDesc typicalMethodDef, InstantiatedType instantiatedType)
        {
            var methodForInstantiatedTypeKey = typicalMethodDef.FullName + ":" + instantiatedType.Instantiation!.ToString();
            if (_methodForInstantiatedTypesByFullName.TryGetValue(methodForInstantiatedTypeKey, out var method))
                return method;

            return _methodForInstantiatedTypesByFullName[methodForInstantiatedTypeKey] = new MethodForInstantiatedType(typicalMethodDef, instantiatedType);
        }
    }
}