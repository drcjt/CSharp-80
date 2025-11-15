using System.Collections;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler
{
    public class LocalVariableTable : IEnumerable<LocalVariableDescriptor>
    {
        private readonly IList<LocalVariableDescriptor> _locals;

        public LocalVariableDescriptor this[int i] => _locals[i];

        public int ParameterCount { get; private set; } = 0;
        public void Add(LocalVariableDescriptor local)
        {
            _locals.Add(local);
            if (local.IsParameter)
            {
                ParameterCount++;
            }
        }

        public int? ReturnBufferArgIndex { get; set; }
        public void Insert(int index, LocalVariableDescriptor local)
        {
            if (local.IsParameter)
            {
                ParameterCount++;
            }
            _locals.Insert(index, local);
        }

        public int Count => _locals.Count;

        public IEnumerator<LocalVariableDescriptor> GetEnumerator() => _locals.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_locals).GetEnumerator();

        public LocalVariableTable()
        {
            _locals = new List<LocalVariableDescriptor>();
        }

        public int GrabTemp(VarType type, int? exactSize)
        {
            LocalVariableDescriptor temp = new()
            {
                IsParameter = false,
                IsTemp = true,
                ExactSize = exactSize ?? 0,
                Type = type
            };

            _locals.Add(temp);

            return _locals.Count - 1;
        }

        public void ResetCount(int count)
        {
            // Remove any variables added after count
            int removeAt = count;
            int removeCount = _locals.Count - count;

            while (removeCount-- > 0)
            {
                _locals.RemoveAt(removeAt);
            }
        }

        public void SetupLocalVariableTable(MethodDesc method)
        {
            int parameterCount = 0;

            if (method.HasThis)
            {
                LocalVariableDescriptor local = new()
                {
                    IsParameter = true,
                    IsTemp = false,
                    Name = "",
                    ExactSize = 2,
                    Type = VarType.Ref,
                };
                Add(local);
                parameterCount++;
            }

            // Setup local variable table - includes parameters as well as locals in method
            for (int parameterIndex = 0; parameterIndex < method.Signature.Length; parameterIndex++)
            {
                MethodParameter parameter = method.Signature[parameterIndex];
                LocalVariableDescriptor local = new()
                {
                    IsParameter = true,
                    IsTemp = false,
                    Name = parameter.Name,
                    ExactSize = parameter.Type.GetElementSize().AsInt,
                    Type = parameter.Type.VarType,
                };
                Add(local);
                parameterCount++;
            }

            foreach (var local in method.Locals)
            {
                LocalVariableDescriptor localVariableDescriptor = new()
                {
                    IsParameter = false,
                    IsTemp = false,
                    Name = local.Name,
                    ExactSize = local.Type.GetElementSize().AsInt,
                    Type = local.Type.VarType,
                };
                Add(localVariableDescriptor);
            }

            if (!method.Signature.ReturnType.IsVoid)
            {
                InitReturnBufferArg(method);
            }
        }

        private void InitReturnBufferArg(MethodDesc method)
        {
            TypeDesc returnType = method.Signature.ReturnType;
            if (returnType.IsValueType && !returnType.IsPrimitive && !returnType.IsEnum)
            {
                TargetDetails target = new(TypeSystem.Common.TargetArchitecture.Z80);

                var returnBuffer = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    Type = VarType.ByRef,
                    IsTemp = false,
                    ExactSize = target.PointerSize,
                };

                // Ensure return buffer parameter goes after the this parameter if present
                ReturnBufferArgIndex = method!.HasThis ? 1 : 0;
                Insert(ReturnBufferArgIndex.Value, returnBuffer);
            }
        }
    }
}
