using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class MethodCompiler
    {
        private IList<LocalVariableDescriptor> _localVariableTable;
        private readonly Compilation _compilation;
        private readonly IConfiguration _configuration;

        public int ParameterCount { get; private set; }

        public ILogger<Compilation> Logger => _compilation.Logger;
        public INameMangler NameMangler => _compilation.NameMangler;

        public MethodCompiler(Compilation compilation, IConfiguration configuration)
        {
            _compilation = compilation;
            _configuration = configuration;
        }

        private void SetupLocalVariableTable(MethodDef method)
        {
            var body = method.MethodBody as CilBody;

            // Setup local variable table - includes parameters as well as locals in method
            _localVariableTable = new List<LocalVariableDescriptor>(method.Parameters.Count + body?.Variables.Count ?? 0);

            var offset = 0;
            for (int parameterIndex = 0; parameterIndex < method.Parameters.Count; parameterIndex++)
            {
                var kind = method.Parameters[parameterIndex].Type.GetStackValueKind();
                var local = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    Kind = kind,
                    // TODO: Should this set field offset??
                    ExactSize = method.Parameters[parameterIndex].Type.GetExactSize(),
                    StackOffset = offset,
                    IsTemp = false,
                    Type = LocalVariableType.Int,
                };
                _localVariableTable.Add(local);
                offset += local.ExactSize;
            }

            if (body != null)
            {
                offset = 0; // Reset as use separate index register to access locals than parameters
                for (int variableIndex = 0; variableIndex < body.Variables.Count; variableIndex++)
                {
                    var kind = body.Variables[variableIndex].Type.GetStackValueKind();
                    var local = new LocalVariableDescriptor()
                    {
                        IsParameter = false,
                        Kind = kind,
                        ExactSize = body.Variables[variableIndex].Type.GetExactSize(true),
                        StackOffset = offset,
                        IsTemp = false,
                        Type = LocalVariableType.Int,
                    };
                    _localVariableTable.Add(local);
                    offset += local.ExactSize;
                }
            }
        }

        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var method = methodCodeNodeNeedingCode.Method;
            ParameterCount = method.Parameters.Count;

            SetupLocalVariableTable(method);

            if (!method.IsIntrinsic() && !method.IsPinvokeImpl)
            {
                var ilImporter = new ILImporter(this, method, _localVariableTable, _configuration);
                var flowgraph = new Flowgraph();
                var codeGenerator = new CodeGenerator(_compilation, _localVariableTable, methodCodeNodeNeedingCode);

                // Main phases of the compiler live here
                var basicBlocks = ilImporter.Import();

                if (_configuration.DumpIRTrees)
                {
                    TreeDumper.Dump(basicBlocks);
                }

                flowgraph.SetBlockOrder(basicBlocks);
                var instructions = codeGenerator.Generate(basicBlocks);
                methodCodeNodeNeedingCode.MethodCode = instructions;
            }
        }
    }
}
