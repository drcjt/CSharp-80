using dnlib.DotNet;
using ILCompiler.IL;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using ILCompiler.TypeSystem.Interop;

namespace ILCompiler.TypeSystem.Dnlib
{
    internal class DnlibMethod : MethodDesc
    {
        private readonly MethodDef _methodDef;
        private readonly DnlibModule _module;
        private readonly ILProvider _ilProvider;
        private MethodIL? _methodIL;
        public DnlibMethod(MethodDef methodDef, DnlibModule module, ILProvider ilProvider)
        {
            _methodDef = methodDef;
            _module = module;
            _ilProvider = ilProvider;
        }

        public override MethodSignature Signature => _module.CreateMethodSignature(_methodDef);

        public override TypeSystemContext Context => _module.Context;

        public override TypeDesc OwningType => (TypeDesc)_module.Create(_methodDef.DeclaringType);

        public override string Name => _methodDef.Name;
        public override string FullName => ToString();

        private const string CompilerIntrinsicAttribute = "System.Runtime.CompilerServices.IntrinsicAttribute";
        public override bool IsIntrinsic => _methodDef.HasCustomAttributes && _methodDef.CustomAttributes.IsDefined(CompilerIntrinsicAttribute);

        public override bool IsPInvoke => _methodDef.IsPinvokeImpl;

        public override PInvokeMetaData? GetPInvokeMetaData()
        {
            if (!IsPInvoke)
                return default;

            var name = _methodDef.ImplMap.Name;
            var module = _methodDef.ImplMap.Module.Name;

            return new PInvokeMetaData(name, module);
        }

        public override bool IsInternalCall => _methodDef.IsInternalCall;
        public override bool IsNewSlot => _methodDef.IsNewSlot;
        public override bool IsAbstract => _methodDef.IsAbstract;
        public override bool IsVirtual => _methodDef.IsVirtual;
        public override bool IsStatic => _methodDef.IsStatic;
        public override bool HasGenericParameters => _methodDef.HasGenericParameters;
        public override bool HasReturnType => _methodDef.HasReturnType;
        public override bool HasThis => _methodDef.HasThis;


        public override IList<LocalVariableDefinition> Locals
        {
            get
            {
                var locals = new List<LocalVariableDefinition>();
                foreach (var local in _methodDef.Body.Variables)
                {
                    var localVariableDefinition = new LocalVariableDefinition(_module.Create(local.Type), local.Name, local.Index);
                    locals.Add(localVariableDefinition);
                }
                return locals;
            }
        }

        public override IList<MethodParameter> Parameters
        {
            get
            {
                var parameters = new List<MethodParameter>();
                foreach (var parameter in _methodDef.Parameters)
                {
                    var methodParameter = new MethodParameter(_module.Create(parameter.Type), parameter.Name);
                    parameters.Add(methodParameter);
                }
                return parameters;
            }
        }

        private void InitializeMethodOverrides()
        {
            _methodOverrides = new MethodImplRecord[_methodDef.Overrides.Count];
            for (int i = 0; i < _methodOverrides.Length; i++)
            {
                var methodBody = _module.Create(_methodDef.Overrides[i].MethodBody);
                var methodDeclaration = _module.Create(_methodDef.Overrides[i].MethodDeclaration);
                _methodOverrides[i] = new MethodImplRecord(methodDeclaration, methodBody);
            }
        }

        private MethodImplRecord[]? _methodOverrides = null;

        public override IEnumerable<MethodImplRecord> Overrides
        {
            get
            {
                if (_methodOverrides == null)
                    InitializeMethodOverrides();

                return _methodOverrides!;
            }
        }

        public override MethodDesc CreateUserMethod(string name)
        {
            return _module.Create(new MethodDefUser(name, _methodDef.MethodSig));
        }

        public override string? GetCustomAttributeValue(string customAttributeName)
        {
            var customAttribute = _methodDef.CustomAttributes.Find(customAttributeName);
            if (customAttribute != null)
            {
                var constructorArguments = customAttribute.ConstructorArguments;
                if (constructorArguments != null)
                {
                    var constructorArgument = constructorArguments[0];
                    object attributeValue = constructorArgument.Value;
                    if (attributeValue != null)
                    {
                        if (attributeValue is UTF8String utf8String)
                            return utf8String.String;
                        else                        
                            return attributeValue.ToString();
                    }
                }
            }

            return null;
        }

        public override MethodIL? MethodIL
        {
            get
            {
                if (_methodIL == null)
                {
                    if (IsIntrinsic)
                    {
                        // Use IL Provider
                        _methodIL = _ilProvider.GetMethodIL(this);
                    }

                    if (_methodIL == null && _methodDef.Body != null)
                    {
                        _methodIL = new DnlibMethodIL(_module, _methodDef.Body);
                    }
                }

                return _methodIL;
            }
        }

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName)
        {
            return _methodDef.HasCustomAttributes && _methodDef.CustomAttributes.IsDefined(attributeNamespace + "." + attributeName);
        }

        public string GetRuntimeExportName()
        {
            var runtimeExportAttribute = _methodDef.CustomAttributes.Find("System.Runtime.RuntimeExportAttribute");
            return runtimeExportAttribute.ConstructorArguments[0].Value.ToString() ?? "";
        }

        public override Instantiation Instantiation
        {
            get
            {
                var genericParams = _methodDef.GenericParameters;
                TypeDesc[] genericParameters = new TypeDesc[genericParams.Count];
                for (int i = 0; i < genericParams.Count; i++)
                {
                    genericParameters[i] = new DnlibGenericParameter(_module, genericParams[i]);
                }
                return new Instantiation(genericParameters);
            }
        }
    }
}