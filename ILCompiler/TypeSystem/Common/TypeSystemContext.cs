namespace ILCompiler.TypeSystem.Common
{
    public class TypeSystemContext
    {        
        public TargetDetails Target { get; } = new TargetDetails(TargetArchitecture.Z80);

        private readonly Dictionary<string, ArrayType> _arrayTypesByFullName = new Dictionary<string, ArrayType>();
        private readonly Dictionary<string, FunctionPointerType> _functionPointerTypesBySignature = new Dictionary<string, FunctionPointerType>();
        private readonly Dictionary<string, PointerType> _pointerTypesByFullName = new Dictionary<string, PointerType>();

        public ModuleDesc? SystemModule { get; set; }

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
    }
}