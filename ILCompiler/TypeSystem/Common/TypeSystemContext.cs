namespace ILCompiler.TypeSystem.Common
{
    public class TypeSystemContext
    {        
        public TargetDetails Target { get; } = new TargetDetails(TargetArchitecture.Z80);

        private readonly Dictionary<string, ArrayType> _arrayTypes = new Dictionary<string, ArrayType>();
        private readonly Dictionary<string, ByRefType> _byRefTypes = new Dictionary<string, ByRefType>();
        private readonly Dictionary<string, FunctionPointerType> _functionPointerTypes = new Dictionary<string, FunctionPointerType>();
        private readonly Dictionary<string, PointerType> _pointerTypes = new Dictionary<string, PointerType>();
        private readonly Dictionary<string, FieldDesc> _fieldForInstantiatedTypes = new Dictionary<string, FieldDesc>();
        private readonly Dictionary<string, MethodDesc> _methodForInstantiatedTypes = new Dictionary<string, MethodDesc>();
        private readonly Dictionary<string, InstantiatedType> _instantiatedTypes = new Dictionary<string, InstantiatedType>();
        private readonly Dictionary<string, InstantiatedMethod> _instantiatedMethods = new Dictionary<string, InstantiatedMethod>();

        public ModuleDesc? SystemModule { get; set; }

        public InstantiatedType GetInstantiatedType(MetadataType typeDef, Instantiation instantiation)
        {
            var key = typeDef.FullName + ":" + instantiation.ToString();
            if (_instantiatedTypes.TryGetValue(key, out var instantiatedType))
                return instantiatedType;

            return _instantiatedTypes[key] = new InstantiatedType(typeDef, instantiation);
        }

        public InstantiatedMethod GetInstantiatedMethod(MethodDesc methodDef, Instantiation instantiation)
        {
            var key = methodDef.FullName + ":" + instantiation.ToString();
            if (_instantiatedMethods.TryGetValue(key, out var instantiatedMethod))
                return instantiatedMethod;

            return _instantiatedMethods[key] = new InstantiatedMethod(methodDef, instantiation);
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

        public ByRefType GetByRefType(TypeDesc parameterType)
        {
            var byrefTypeKey = parameterType.FullName;
            if (_byRefTypes.TryGetValue(byrefTypeKey, out var byrefType))
                return byrefType;

            return _byRefTypes[byrefTypeKey] = new ByRefType(parameterType);
        }

        public ArrayType GetArrayType(TypeDesc elementType, int rank)
        {
            var arrayTypeKey = $"{elementType.FullName}_{rank}";
            if (_arrayTypes.TryGetValue(arrayTypeKey, out var _array))
                return _array;

            return _arrayTypes[arrayTypeKey] = new ArrayType(elementType, rank);
        }

        public FunctionPointerType GetFunctionPointerType(MethodSignature signature)
        {
            if (_functionPointerTypes.TryGetValue(signature.ToString(), out var functionPointer))
                return functionPointer;

            return _functionPointerTypes[signature.ToString()] = new FunctionPointerType(signature);
        }

        public PointerType GetPointerType(TypeDesc parameterType)
        {
            if (_pointerTypes.TryGetValue(parameterType.FullName, out var pointer))
                return pointer;

            return _pointerTypes[parameterType.FullName] = new PointerType(parameterType);
        }

        public FieldDesc GetFieldForInstantiatedType(FieldDesc fieldDef, InstantiatedType instantiatedType)
        {
            // TODO: Fix key generation as fullname/instantiation may contain colons
            var key = fieldDef.ToString() + ":" + instantiatedType.Instantiation!.ToString();
            if (_fieldForInstantiatedTypes.TryGetValue(key, out var field)) 
                return field; 

            return _fieldForInstantiatedTypes[key] = new FieldForInstantiatedType(fieldDef, instantiatedType);
        }

        public MethodDesc GetMethodForInstantiatedType(MethodDesc typicalMethodDef, InstantiatedType instantiatedType)
        {
            var methodForInstantiatedTypeKey = typicalMethodDef.FullName + ":" + instantiatedType.Instantiation!.ToString();
            if (_methodForInstantiatedTypes.TryGetValue(methodForInstantiatedTypeKey, out var method))
                return method;

            return _methodForInstantiatedTypes[methodForInstantiatedTypeKey] = new MethodForInstantiatedType(typicalMethodDef, instantiatedType);
        }
    }
}