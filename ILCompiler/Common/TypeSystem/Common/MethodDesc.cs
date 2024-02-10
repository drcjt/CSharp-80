using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Common.TypeSystem.Common
{
    public class MethodDesc
    {
        protected MethodDef _methodDef;

        public MethodDesc(MethodDef methodDef)
        {
            _methodDef = methodDef;
            Body = methodDef.Body;
        }

        public bool LocallocUsed { get; set; } = false;
        public bool IsIntrinsic => _methodDef.IsIntrinsic();

        public bool IsPInvokeImpl => _methodDef.IsPinvokeImpl;
        public bool IsInternalCall => _methodDef.IsInternalCall;

        public bool IsStaticConstructor => _methodDef.IsStaticConstructor;

        public bool IsInstanceConstructor => _methodDef.IsInstanceConstructor;

        public bool IsStatic => _methodDef.IsStatic;

        public virtual TypeSig ResolveType(TypeSig type) => type;

        public ParameterList ParameterList => _methodDef.Parameters;

        //public virtual IList<Parameter> Parameters => _methodDef.Parameters.ToList<Parameter>();

        public virtual IList<Parameter> Parameters() => _methodDef.Parameters.ToList<Parameter>();

        public virtual IList<Local> Locals() => _methodDef.Body.Variables.ToList<Local>();

        public MethodBody MethodBody => _methodDef.MethodBody;

        public bool HasReturnType => _methodDef.HasReturnType;

        public virtual TypeSig ReturnType => _methodDef.ReturnType;

        public bool HasThis => _methodDef.HasThis;

        public virtual string FullName => _methodDef.FullName;

        public TypeDef DeclaringType => _methodDef.DeclaringType;

        public string Name => _methodDef.Name;

        public CilBody Body { get; set; }

        public bool HasExceptionHandlers => Body.ExceptionHandlers.Count > 0;

        public bool HasCustomAttribute(string attributeNamespace, string attributeName) => _methodDef.HasCustomAttribute(attributeNamespace, attributeName);

        public CustomAttribute FindCustomAttribute(string attributeName) => _methodDef.CustomAttributes.Find(attributeName);
    }
}
