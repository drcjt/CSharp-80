﻿using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.Dnlib
{
    public class DnlibModule : ModuleDesc
    {
        private readonly Dictionary<string, FieldDesc> _fieldsByFullName = new Dictionary<string, FieldDesc>();
        private readonly Dictionary<string, DefType> _defTypesByFullName = new Dictionary<string, DefType>();
        private readonly Dictionary<string, DnlibMethod> _dnlibMethodsByFullName = new Dictionary<string, DnlibMethod>();

        private readonly CorLibModuleProvider _corLibModuleProvider;

        public DnlibModule(TypeSystemContext context, CorLibModuleProvider corLibModuleProvider) : base(context)
        {
            _corLibModuleProvider = corLibModuleProvider;
            context.SystemModule = this;
        }

        public static DnlibModule Create(TypeSystemContext context, CorLibModuleProvider corLibModuleProvider)
        {
            return new DnlibModule(context, corLibModuleProvider);
        }

        public FieldDesc Create(IField field)
        {
            var resolvedFieldDef = field.ResolveFieldDefThrow();
            if (!_fieldsByFullName.TryGetValue(resolvedFieldDef.FullName, out FieldDesc? fieldDesc))
            {
                fieldDesc = new DnlibField(resolvedFieldDef, this);
                _fieldsByFullName[resolvedFieldDef.FullName] = fieldDesc;
            }

            return fieldDesc;
        }

        public TypeDesc Create(TypeSig typeSig, Instantiation? instantiation = null)
        {
            if (typeSig.IsSZArray)
            {
                var elemTypeSig = typeSig.Next;
                var elemTypeDesc = Create(elemTypeSig);
                return Context.GetArrayType(elemTypeDesc, -1);
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
                return new PointerType(parameterType);
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
                TypeDesc genericMethodParameter = new SignatureMethodVariable(Context, (int)((GenericSig)typeSig).Number);
                if (instantiation != null)
                {
                    genericMethodParameter = genericMethodParameter.InstantiateSignature(null, instantiation);
                }
                return genericMethodParameter;
            }

            throw new NotImplementedException();
        }

        public MethodDesc Create(IMethod methodDefOrRef)
        {
            if (methodDefOrRef is MethodSpec methodSpec)
            {
                var methodDef = Create(methodSpec.ResolveMethodDefThrow());

                var genericInstMethodSig = methodSpec.GenericInstMethodSig;
                var genericParams = genericInstMethodSig.GenericArguments;
                TypeDesc[] genericParameters = new TypeDesc[genericParams.Count];
                for (int i = 0; i < genericParams.Count; i++)
                {
                    genericParameters[i] = Create(genericParams[i]);
                }
                var instantiation = new Instantiation(genericParameters);

                var instantiatedMethod = new InstantiatedMethod(methodDef, instantiation);
                return instantiatedMethod;
            }
            else
            {
                var methodDef = methodDefOrRef.ResolveMethodDefThrow();

                if (!_dnlibMethodsByFullName.TryGetValue(methodDef.FullName, out DnlibMethod? methodDesc))
                {
                    methodDesc = new DnlibMethod(methodDef, this);
                    _dnlibMethodsByFullName[methodDef.FullName] = methodDesc;
                }

                return methodDesc;
            }
        }

        public TypeDesc Create(ITypeDefOrRef typeDefOrRef, Instantiation? instantiation = null)
        {
            if (typeDefOrRef is TypeDef td)
            {
                if (td.ContainsGenericParameter)
                {
                    return new InstantiatedType((MetadataType)Create(td));
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
                var szArraySig = ts.TryGetSZArraySig();
                if (szArraySig != null)
                {
                    return Create(szArraySig);
                }
                var genericSig = ts.TryGetGenericSig();
                if (genericSig != null)
                {
                    return Create(genericSig, instantiation);
                }
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

            return new MethodSignature(!methodSig.HasThis, Create(methodSig.RetType), parameters.ToArray());
        }

        public MethodSignature CreateMethodSignature(MethodDef methodDef)
        {
            var parameters = new List<MethodParameter>();
            foreach (var parameter in methodDef.Parameters)
            {
                parameters.Add(new MethodParameter(Create(parameter.Type), parameter.Name));
            }

            return new MethodSignature(!methodDef.HasThis, Create(methodDef.ReturnType), parameters.ToArray());
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
    }
}