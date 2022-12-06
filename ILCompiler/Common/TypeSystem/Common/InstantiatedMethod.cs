using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ILCompiler.Common.TypeSystem.Common
{
    public class InstantiatedMethod : MethodDesc
    {
        private IList<TypeSig> _genericParameters;
        private string _fullName;

        public InstantiatedMethod(MethodDef methodDef, IList<TypeSig> genericParameters, string fullName) : base(methodDef)
        {
            _genericParameters = genericParameters;
            _fullName = fullName;
        }

        public override string FullName => _fullName;

        public override TypeSig ReturnType => GenericTypeInstantiator.Instantiate(_methodDef.ReturnType, _genericParameters);

        public override TypeSig ResolveType(TypeSig type)
        {
            return GenericTypeInstantiator.Instantiate(type, _genericParameters);
        }

        public override IList<Local> Locals
        {
            get
            {
                var instantiatedLocals = new List<Local>();
                foreach (var local in _methodDef.Body.Variables)
                {
                    var instantiatedType = GenericTypeInstantiator.Instantiate(local.Type, _genericParameters);
                    var instantiatedLocal = new Local(instantiatedType, local.Name, local.Index);
                    instantiatedLocals.Add(instantiatedLocal);
                }
                return instantiatedLocals;
            }
        }

        public override IList<Parameter> Parameters
        {
            get
            {
                var instantiatedParameters = new List<Parameter>();
                foreach (var param in _methodDef.Parameters)
                {
                    var instantiatedType = GenericTypeInstantiator.Instantiate(param.Type, _genericParameters);
                    var instantiatedParameter = new Parameter(param.Index, instantiatedType);
                    instantiatedParameters.Add(instantiatedParameter);
                }

                return instantiatedParameters;
            }
        }
    }
}
