using ILCompiler.Compiler;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.RuntimeDetermined;

namespace ILCompiler.TypeSystem.Common
{
    public class TypeSystemContext
    {   
        public IConfiguration Configuration { get; init; }
        public TypeSystemContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TargetDetails Target { get; } = new TargetDetails(TargetArchitecture.Z80);

        private readonly Dictionary<string, ArrayType> _arrayTypes = new Dictionary<string, ArrayType>();
        private readonly Dictionary<string, FunctionPointerType> _functionPointerTypes = new Dictionary<string, FunctionPointerType>();
        private readonly Dictionary<string, PointerType> _pointerTypes = new Dictionary<string, PointerType>();
        private readonly Dictionary<string, FieldDesc> _fieldForInstantiatedTypes = new Dictionary<string, FieldDesc>();
        private readonly Dictionary<string, MethodDesc> _methodForInstantiatedTypes = new Dictionary<string, MethodDesc>();
        private readonly Dictionary<string, InstantiatedType> _instantiatedTypes = new Dictionary<string, InstantiatedType>();
        private readonly Dictionary<string, InstantiatedMethod> _instantiatedMethods = new Dictionary<string, InstantiatedMethod>();
        private readonly Dictionary<string, ByRefType> _byRefTypes = new Dictionary<string, ByRefType>();
        private readonly Dictionary<string, RuntimeDeterminedType> _runtimeDeterminedTypes = new Dictionary<string, RuntimeDeterminedType>();
        private readonly Dictionary<string, MethodForRuntimeDeterminedType> _methodForRuntimeDeterminedTypes = new Dictionary<string, MethodForRuntimeDeterminedType>();
        private readonly Dictionary<uint, SignatureVariable> _signatureVariables = new Dictionary<uint, SignatureVariable>();
        public ModuleDesc? SystemModule { get; set; }

        public SharedGenericsMode GenericsMode { get; set; } = SharedGenericsMode.Disabled;

        public void SetSystemModule(ModuleDesc moduleDesc)
        {
            SystemModule = moduleDesc;

            // Preload well known types
            MetadataType type = SystemModule.GetType("System", "Nullable`1");
            type.SetWellKnownType(WellKnownType.Nullable);
        }

        public bool ComputeHasGcStaticBase(FieldDesc field)
        {
            TypeDesc fieldType = field.FieldType;
            if (fieldType.IsValueType)
                return ((DefType)fieldType).ContainsGcPointers;
            else
            {
                return fieldType.IsGcPointer;

            }
        }

        public InstantiatedType GetInstantiatedType(MetadataType typeDef, Instantiation instantiation)
        {
            var newInstantiatedType = new InstantiatedType(typeDef, instantiation);
            var key = newInstantiatedType.FullName;
            if (_instantiatedTypes.TryGetValue(key, out var instantiatedType))
                return instantiatedType;

            return _instantiatedTypes[key] = newInstantiatedType;
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
                if (!type.IsArrayTypeWithoutGenericInterfaces())
                {
                    MetadataType arrayShadowType = SystemModule!.GetType("System", "Array`1");
                    return arrayShadowType.MakeInstantiatedType(((ArrayType)type).ElementType);
                }

                return GetWellKnownType(WellKnownType.Array);
            }

            return ((DefType)type);
        }

        public ArrayType GetArrayType(TypeDesc elementType) => GetArrayType(elementType, -1);

        public ArrayType GetArrayType(TypeDesc elementType, int rank)
        {
            var arrayTypeKey = $"{elementType.FullName}_{rank}";
            if (_arrayTypes.TryGetValue(arrayTypeKey, out var _array))
                return _array;

            return _arrayTypes[arrayTypeKey] = new ArrayType(elementType, rank);
        }

        public SignatureVariable GetSignatureVariable(int index, bool method)
        {
            uint combinedIndex = method ? ((uint) index | 0x80000000) : (uint)index;
            if (_signatureVariables.TryGetValue(combinedIndex, out var signatureVariable))
                return signatureVariable;

            return _signatureVariables[combinedIndex] = method ? new SignatureMethodVariable(this, index) : new SignatureTypeVariable(this, index);
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

        public ByRefType GetByRefType(TypeDesc byRefType)
        {
            if (_byRefTypes.TryGetValue(byRefType.FullName, out var byRef))
                return byRef;

            return _byRefTypes[byRefType.FullName] = new ByRefType(byRefType);
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

        public RuntimeDeterminedType GetRuntimeDeterminedType(DefType plainCanonType, GenericParameterDesc detailsType)
        {
            var runtimeDeterminedTypeKey = plainCanonType.FullName + ":" + detailsType.ToString();
            if (_runtimeDeterminedTypes.TryGetValue(runtimeDeterminedTypeKey, out var runtimeDeterminedType))
                return runtimeDeterminedType;

            return _runtimeDeterminedTypes[runtimeDeterminedTypeKey] = new RuntimeDeterminedType(plainCanonType, detailsType);
        }

        public MethodDesc GetMethodForRuntimeDeterminedType(MethodDesc typicalMethodDef, RuntimeDeterminedType rdType)
        {
            var methodForRDTypeKey = typicalMethodDef.FullName + ":" + rdType.CanonicalType.FullName;
            if (_methodForRuntimeDeterminedTypes.TryGetValue(methodForRDTypeKey, out var methodForRDType))
                return methodForRDType;

            return _methodForRuntimeDeterminedTypes[methodForRDTypeKey] = new MethodForRuntimeDeterminedType(typicalMethodDef, rdType);
        }

        public Instantiation ConvertInstantiationToCanonForm(Instantiation instantiation, CanonicalFormKind kind, out bool changed)
        {
            if (GenericsMode == SharedGenericsMode.CanonicalReferenceTypes)
            {
                return RuntimeDeterminedCanonicalizationAlgorithm.ConvertInstantiationToCanonForm(instantiation, kind, out changed);
            }

            changed = false;
            return instantiation;
        }

        public TypeDesc ConvertToCanon(TypeDesc typeToConvert, CanonicalFormKind kind)
        {
            if (GenericsMode == SharedGenericsMode.CanonicalReferenceTypes)
            {
                return RuntimeDeterminedCanonicalizationAlgorithm.ConvertToCanon(typeToConvert, kind);
            }

            return typeToConvert;
        }

        public TypeDesc ConvertToCanon(TypeDesc typeToConvert, ref CanonicalFormKind kind)
        {
            if (GenericsMode == SharedGenericsMode.CanonicalReferenceTypes)
                return RuntimeDeterminedCanonicalizationAlgorithm.ConvertToCanon(typeToConvert, ref kind);

            return typeToConvert;
        }

        private CanonType? _canonType;
        public CanonBaseType CanonType
        {
            get
            {
                _canonType ??= new CanonType(this);
                return _canonType;
            }
        }

        public bool IsCanonicalDefinitionType(TypeDesc type, CanonicalFormKind kind) => kind switch
        {
            CanonicalFormKind.Any => type == CanonType,
            CanonicalFormKind.Specific => type == CanonType,
            _ => false,
        };

        public DefType GetWellKnownType(WellKnownType wellKnownType)
        {
            const string SystemNameSpace = "System";
            switch (wellKnownType)
            {
                case WellKnownType.String:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "String");
                case WellKnownType.Char:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "Char");
                case WellKnownType.Object:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "Object");
                case WellKnownType.Array:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "Array");
                case WellKnownType.Int32:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "Int32");
                case WellKnownType.IntPtr:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "IntPtr");
                case WellKnownType.Void:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "Void");
                case WellKnownType.Nullable:
                    return (DefType)SystemModule!.GetType(SystemNameSpace, "Nullable`1");
                default:
                    throw new TypeLoadException();
            }
        }
    }

    public enum SharedGenericsMode
    {
        Disabled,
        CanonicalReferenceTypes,
    }
}
