using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Common.TypeSystem.Common.dnlib
{
    internal class DnlibMethod : MethodDesc
    {
        private MethodDef _methodDef;
        private TypeSystemContext _typeSystemContext;
        public DnlibMethod(MethodDef methodDef, TypeSystemContext typeSystemContext)
        {
            _methodDef = methodDef;
            _typeSystemContext = typeSystemContext;
            Body = _methodDef.Body;
        }

        public override MethodSignature Signature => Context.CreateMethodSignature(_methodDef);

        public override TypeSystemContext Context => _typeSystemContext;

        public override TypeDesc OwningType => (TypeDesc)Context.Create(_methodDef.DeclaringType);

        public override string Name => _methodDef.Name;
        public override string FullName => _methodDef.FullName;

        public override bool IsIntrinsic => _methodDef.IsIntrinsic();
        public override bool IsPInvoke => _methodDef.IsPinvokeImpl;
        public override string PInvokeMethodName => _methodDef.ImplMap.Name;
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
                    var localVariableDefinition = new LocalVariableDefinition(Context.Create(local.Type), local.Name, local.Index);
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
                    var methodParameter = new MethodParameter(Context.Create(parameter.Type), parameter.Name);
                    parameters.Add(methodParameter);
                }
                return parameters;
            }
        }

        public override IList<MethodOverride> Overrides => _methodDef.Overrides;
        public override MethodSig MethodSig => _methodDef.MethodSig;
        public override CustomAttributeCollection CustomAttributes => _methodDef.CustomAttributes;


        public override CilBody Body { get; set; }

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName) => _methodDef.HasCustomAttribute(attributeNamespace, attributeName);

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
                    genericParameters[i] = new DnlibGenericParameter(Context, genericParams[i]);
                }
                return new Instantiation(genericParameters);
            }
        }
    }
}
