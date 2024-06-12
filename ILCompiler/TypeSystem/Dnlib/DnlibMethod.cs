﻿using dnlib.DotNet;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Dnlib
{
    internal class DnlibMethod : MethodDesc
    {
        private readonly MethodDef _methodDef;
        private readonly DnlibModule _module;
        private readonly MethodIL? _methodIL;
        public DnlibMethod(MethodDef methodDef, DnlibModule module)
        {
            _methodDef = methodDef;
            _module = module;
            if (_methodDef.Body != null)
            {
                _methodIL = new DnlibMethodIL(module, _methodDef.Body);
            }
        }

        public override MethodSignature Signature => _module.CreateMethodSignature(_methodDef);

        public override TypeSystemContext Context => _module.Context;

        public override TypeDesc OwningType => (TypeDesc)_module.Create(_methodDef.DeclaringType);

        public override string Name => _methodDef.Name;
        public override string FullName => ToString();

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

        public override IList<MethodOverride> Overrides => _methodDef.Overrides;
        public override MethodSig MethodSig => _methodDef.MethodSig;
        public override CustomAttributeCollection CustomAttributes => _methodDef.CustomAttributes;


        public override MethodIL? MethodIL => _methodIL;

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
                    genericParameters[i] = new DnlibGenericParameter(_module, genericParams[i]);
                }
                return new Instantiation(genericParameters);
            }
        }
    }
}