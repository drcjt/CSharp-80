using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace ILCompiler.Compiler
{
    public class MethodCompiler
    {
        private LocalVariableDescriptor[] _localVariableTable;
        private readonly Compilation _compilation;

        public int ParameterCount { get; private set; }

        public ILogger<Compilation> Logger => _compilation.Logger;
        public IConfiguration Configuration => _compilation.Configuration;
        public INameMangler NameMangler => _compilation.NameMangler;

        public MethodCompiler(Compilation compilation)
        {
            _compilation = compilation;
        }

        private void SetupLocalVariableTable(MethodDef method)
        {
            var body = method.MethodBody as CilBody;

            // Setup local variable table - includes parameters as well as locals in method
            _localVariableTable = new LocalVariableDescriptor[method.Parameters.Count + body?.Variables.Count ?? 0];

            for (int parameterIndex = 0; parameterIndex < method.Parameters.Count; parameterIndex++)
            {
                var kind = method.Parameters[parameterIndex].Type.GetStackValueKind();
                var local = new LocalVariableDescriptor() 
                { 
                    IsParameter = true, 
                    Kind = kind,
                    ExactSize = GetExactSize(kind)
                };
                _localVariableTable[parameterIndex] = local;
            }

            if (body != null)
            {
                for (int variableIndex = 0; variableIndex < body.Variables.Count; variableIndex++)
                {
                    var kind = body.Variables[variableIndex].Type.GetStackValueKind();
                    var local = new LocalVariableDescriptor() 
                    { 
                        IsParameter = false, 
                        Kind = kind, 
                        ExactSize = GetExactSize(kind) 
                    };
                    _localVariableTable[method.Parameters.Count + variableIndex] = local;
                }
            }
        }

        // TODO: Move this to a better place e.g TypeList.cs
        private int GetExactSize(StackValueKind kind)
        {
            switch (kind)
            {
                case StackValueKind.Int16:
                    return 2;
                case StackValueKind.Int32:
                    return 4;
                case StackValueKind.Int64:
                    return 8;
                case StackValueKind.ObjRef:
                    return 2;
                case StackValueKind.NativeInt:
                    return 2;
                default:
                    throw new NotImplementedException($"Kind {kind} not yet supported");
            }
        }

        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var method = methodCodeNodeNeedingCode.Method;
            ParameterCount = method.Parameters.Count;

            SetupLocalVariableTable(method);

            if (!method.IsConstructor && !method.IsIntrinsic() && !method.IsPinvokeImpl)
            {
                var ilImporter = new ILImporter(this, method);
                var flowgraph = new Flowgraph();
                var codeGenerator = new CodeGenerator(_compilation, _localVariableTable, methodCodeNodeNeedingCode);

                // Main phases of the compiler live here
                var basicBlocks = ilImporter.Import();
                flowgraph.SetBlockOrder(basicBlocks);
                var instructions = codeGenerator.Generate(basicBlocks);
                methodCodeNodeNeedingCode.MethodCode = instructions;
            }
        }
    }
}
