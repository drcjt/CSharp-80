using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.IL;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.Dnlib
{
    public class DnlibModule : ModuleDesc
    {
        private readonly Dictionary<string, FieldDesc> _fieldsByFullName = new Dictionary<string, FieldDesc>();
        private readonly Dictionary<string, DefType> _defTypesByFullName = new Dictionary<string, DefType>();
        private readonly Dictionary<string, MethodDesc> _dnlibMethodsByFullName = new Dictionary<string, MethodDesc>();

        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly ILProvider _ilProvider;

        public ILProvider ILProvider => _ilProvider;

        public DnlibModule(TypeSystemContext context, CorLibModuleProvider corLibModuleProvider, RTILProvider ilProvider) : base(context)
        {
            _corLibModuleProvider = corLibModuleProvider;
            context.SystemModule = this;
            _ilProvider = ilProvider;
        }

        public FieldDesc Create(IField field)
        {
            var resolvedFieldDef = field.ResolveFieldDefThrow();
            if (!_fieldsByFullName.TryGetValue(resolvedFieldDef.FullName, out FieldDesc? fieldDesc))
            {
                fieldDesc = new DnlibField(resolvedFieldDef, this);
                _fieldsByFullName[resolvedFieldDef.FullName] = fieldDesc;
            }

            if (field.DeclaringType.TryGetGenericInstSig() != null)
            {
                var typeSig = field.DeclaringType.ToTypeSig();

                var instantiatedType = ResolveGenericInstanceType(typeSig);
                fieldDesc = Context.GetFieldForInstantiatedType(fieldDesc, instantiatedType);
            }

            return fieldDesc;
        }

        public TypeDesc Create(TypeSig typeSig)
        {
            if (typeSig.IsSZArray)
            {
                var elemTypeSig = typeSig.Next;
                var elemTypeDesc = Create(elemTypeSig);
                return elemTypeDesc.MakeArrayType();
            }
            if (typeSig.IsArray)
            {
                var arraySig = typeSig.ToArraySig();
                var elemTypeSig = typeSig.Next;
                var elemTypeDesc = Create(elemTypeSig);
                return elemTypeDesc.MakeArrayType((int)arraySig.Rank);
            }
            if (typeSig.IsFunctionPointer)
            {
                var fnPtrSig = (FnPtrSig)typeSig;
                return Context.GetFunctionPointerType(Create(fnPtrSig.MethodSig));
            }

            if (typeSig.IsTypeDefOrRef)
            {
                var typeDefOrRef = typeSig.TryGetTypeDefOrRef();
                var resolvedTypeDefOrRef = typeDefOrRef.ResolveTypeDef();
                return Create((ITypeDefOrRef)resolvedTypeDefOrRef);
            }

            if (typeSig.IsPointer)
            {
                var parameterTypeSig = typeSig.Next;
                var parameterType = Create(parameterTypeSig);
                return Context.GetPointerType(parameterType);
            }

            if (typeSig.IsPinned)
            {
                var pinnedTypeSig = (PinnedSig)typeSig;
                return Create(pinnedTypeSig.Next);
            }

            if (typeSig.IsByRef)
            {
                var byRefTypeSig = (ByRefSig)typeSig;
                return new ByRefType(Create(byRefTypeSig.Next));
            }

            if (typeSig.IsGenericMethodParameter)
            {
                TypeDesc genericMethodParameter = Context.GetSignatureVariable((int)((GenericSig)typeSig).Number, method:true);
                return genericMethodParameter;
            }

            if (typeSig.IsGenericInstanceType)
            {
                return ResolveGenericInstanceType(typeSig);
            }

            if (typeSig.IsGenericTypeParameter)
            {
                TypeDesc genericTypeParameter = Context.GetSignatureVariable((int)((GenericVar)typeSig).Number, method:false);
                return genericTypeParameter;
            }

            // TODO: Is this needed?
            if (typeSig is GenericVar genericVar)
            {
                var genericParameter = new DnlibGenericParameter(this, genericVar.GenericParam);
                return genericParameter;
            }

            if (typeSig is CModReqdSig reqdSig)
            {
                return Create(reqdSig.Next);
            }

            throw new NotImplementedException();
        }

        public MethodDesc Create(IMethod methodDefOrRef)
        {
            if (methodDefOrRef is MethodSpec methodSpec)
            {
                return ResolveMethodSpecification(methodSpec);
            }
            else if (methodDefOrRef is MemberRef memberRef)
            {
                return ResolveMemberReference(memberRef);
            }
            else if (methodDefOrRef is MethodDef methodDef)
            {
                if (!_dnlibMethodsByFullName.TryGetValue(methodDef.FullName, out MethodDesc? methodDesc))
                {
                    methodDesc = new DnlibMethod(methodDef, this, _ilProvider);
                    _dnlibMethodsByFullName[methodDefOrRef.FullName] = methodDesc;
                }

                if (methodDef.DeclaringType.TryGetGenericInstSig() != null)
                {
                    var typeSig = methodDef.DeclaringType.ToTypeSig();
                    var instantiatedType = ResolveGenericInstanceType(typeSig);
                    methodDesc = Context.GetMethodForInstantiatedType(methodDesc, instantiatedType);
                }

                return methodDesc;
            }

            throw new NotImplementedException();
        }

        public TypeDesc Create(ITypeDefOrRef typeDefOrRef)
        {
            if (typeDefOrRef is TypeDef td)
            {
                if (td.ContainsGenericParameter)
                {
                    throw new NotImplementedException("Open generic types not yet supported");
                }
                else
                {
                    if (!_defTypesByFullName.TryGetValue(td.FullName, out DefType? defType))
                    {
                        defType = new DnlibType(td, this);
                        _defTypesByFullName[td.FullName] = defType;
                    }

                    return defType;
                }
            }

            if (typeDefOrRef is TypeRef tr)
            {
                td = tr.ResolveThrow();
                return Create((ITypeDefOrRef)td);
            }

            if (typeDefOrRef is TypeSpec ts)
            {
                var genericInstSig = ts.TryGetGenericInstSig();
                if (genericInstSig != null)
                {
                    return ResolveGenericInstanceType(genericInstSig);
                }

                var szArraySig = ts.TryGetSZArraySig();
                if (szArraySig != null)
                {
                    return Create(szArraySig);
                }
                var genericSig = ts.TryGetGenericSig();
                if (genericSig != null)
                {
                    return Create(genericSig);
                }

                var typeDefOrRefSig = ts.TryGetTypeDefOrRefSig();
                if (typeDefOrRefSig != null)
                {
                    return Create(typeDefOrRefSig);
                }

                var ptrSig = ts.TryGetPtrSig();
                if (ptrSig != null)
                {
                    return Create(ptrSig);
                }

                // TODO: should this also check for other sig types?
            }

            return Create(typeDefOrRef);
        }

        public MethodSignature Create(MethodSig methodSig)
        {
            var parameters = new List<MethodParameter>();
            foreach (var parameter in methodSig.Params)
            {
                parameters.Add(new MethodParameter(Create(parameter), parameter.GetName()));
            }

            MethodSignatureFlags flags = 0;
            if (!methodSig.HasThis)
                flags |= MethodSignatureFlags.Static;

            if (methodSig.ExplicitThis)
                flags |= MethodSignatureFlags.ExplicitThis;

            return new MethodSignature(flags, Create(methodSig.RetType), parameters.ToArray());
        }

        private MethodDesc ResolveMemberReference(MemberRef memberReference)
        {
            TypeDesc? parentTypeDesc = Create(memberReference.DeclaringType.ToTypeSig());

            string name = memberReference.Name.String;

            MethodSignature? methodSig = Create(memberReference.MethodSig);
            TypeDesc? typeDescToInspect = parentTypeDesc;
            Instantiation? instantiation = null;

            // Try to resolve the name and signature in the current type or any of the base types
            do
            {
                MethodDesc? method = typeDescToInspect.GetMethod(name, methodSig, instantiation);
                if (method != null)
                {
                    // Instance constructors are not inherited
                    if (typeDescToInspect != parentTypeDesc && method.IsConstructor)
                        break;

                    return method;
                }

                var baseType = typeDescToInspect.BaseType;
                if (baseType != null)
                {
                    // handle generic base types
                    Instantiation? newInstantiation = typeDescToInspect.GetTypeDefinition().BaseType?.Instantiation;
                    if (instantiation is not null && newInstantiation is not null)
                    {
                        TypeDesc[] newSubstitutionTypes = new TypeDesc[newInstantiation.Length];
                        for (int i = 0; i < newInstantiation.Length; i++)
                        {
                            newSubstitutionTypes[i] = newInstantiation[i].InstantiateSignature(instantiation, default(Instantiation));
                        }
                        newInstantiation = new Instantiation(newSubstitutionTypes);
                    }
                    instantiation = newInstantiation;
                }
                typeDescToInspect = baseType;
            } while (typeDescToInspect != null);

            throw new Exception($"Missing Method Failure {name}, {methodSig}");
        }

        public MethodSignature CreateMethodSignature(MethodDef methodDef)
        {
            var parameters = new List<MethodParameter>();
            foreach (var parameter in methodDef.Parameters)
            {
                if (!parameter.IsHiddenThisParameter)
                {
                    parameters.Add(new MethodParameter(Create(parameter.Type), parameter.Name));
                }
            }

            MethodSignatureFlags flags = 0;
            if (!methodDef.HasThis)
                flags |= MethodSignatureFlags.Static;
            if (methodDef.ExplicitThis)
                flags |= MethodSignatureFlags.ExplicitThis;

            return new MethodSignature(flags, Create(methodDef.ReturnType), parameters.ToArray());
        }

        public TypeSystemEntity CreateFromTypeOrMethodDef(ITypeOrMethodDef typeOrMethodDef)
        {
            if (typeOrMethodDef is TypeDef)
            {
                return Create((ITypeDefOrRef)typeOrMethodDef);
            }
            else
            {
                return Create((IMethodDefOrRef)typeOrMethodDef);
            }
        }

        public override object GetType(string nameSpace, string name)
        {
            return Create(_corLibModuleProvider.FindThrow($"{nameSpace}.{name}"));
        }

        private InstantiatedType ResolveGenericInstanceType(TypeSig typeSig)
        {
            var genericType = typeSig.ToGenericInstSig().GenericType;
            var genericParams = typeSig.ToGenericInstSig().GenericArguments;
            TypeDesc[] genericParameters = new TypeDesc[genericParams.Count];
            for (int i = 0; i < genericParams.Count; i++)
            {
                genericParameters[i] = Create(genericParams[i]);
            }
            var instantiation = new Instantiation(genericParameters);
            MetadataType metadataType = (MetadataType)Create(genericType);

            return Context.GetInstantiatedType(metadataType, instantiation);
        }

        private InstantiatedMethod ResolveMethodSpecification(MethodSpec methodSpec)
        {
            var genericInstMethodSig = methodSpec.GenericInstMethodSig;
            var genericParams = genericInstMethodSig.GenericArguments;
            TypeDesc[] genericParameters = new TypeDesc[genericParams.Count];
            for (int i = 0; i < genericParams.Count; i++)
            {
                genericParameters[i] = Create(genericParams[i]);
            }
            var instantiation = new Instantiation(genericParameters);
            var methodDef = Create(methodSpec.ResolveMethodDefThrow());

            return Context.GetInstantiatedMethod(methodDef, instantiation);
        }
    }
}