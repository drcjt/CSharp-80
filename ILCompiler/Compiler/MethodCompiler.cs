using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.IoC;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;

namespace ILCompiler.Compiler
{
    public class MethodCompiler : IMethodCompiler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MethodCompiler> _logger;
        private readonly Factory<ICodeGenerator> _codeGeneratorFactory;
        private readonly Factory<IILImporter> _ilImporterFactory;

        private int _parameterCount;
        private int? _returnBufferArgIndex;
        
        private IList<LocalVariableDescriptor> _localVariableTable;

        public MethodCompiler(ILogger<MethodCompiler> logger, IConfiguration configuration, Factory<ICodeGenerator> codeGeneratorFactory, Factory<IILImporter> ilImporterFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _codeGeneratorFactory = codeGeneratorFactory;
            _ilImporterFactory = ilImporterFactory;
            _localVariableTable = new List<LocalVariableDescriptor>();
        }

        private void SetupLocalVariableTable(MethodDef method)
        {
            var body = method.MethodBody as CilBody;

            // Setup local variable table - includes parameters as well as locals in method
            for (int parameterIndex = 0; parameterIndex < method.Parameters.Count; parameterIndex++)
            {
                var kind = method.Parameters[parameterIndex].Type.GetStackValueKind();
                var local = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    Kind = kind,
                    IsTemp = false,
                    Name = method.Parameters[parameterIndex].Name,
                    ExactSize = method.Parameters[parameterIndex].Type.GetExactSize(),
                };
                _localVariableTable.Add(local);
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
                        IsTemp = false,
                        Name = body.Variables[variableIndex].Name,
                        ExactSize =  body.Variables[variableIndex].Type.GetExactSize(),
                    };
                    _localVariableTable.Add(local);
                }
            }

            if (method.HasReturnType)
            {
                var returnType = method.ReturnType;
                InitReturnBufferArg(returnType, method.HasThis);
            }
        }

        private void InitReturnBufferArg(TypeSig returnType, bool hasThis)
        {
            if (returnType.IsStruct())
            {
                var returnBuffer = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    Kind = StackValueKind.ByRef,
                    IsTemp = false,
                    ExactSize = 4, // Ptr is just an int32
                };

                // Ensure return buffer parameter goes after the this parameter if present
                _returnBufferArgIndex = hasThis ? 1 : 0;
                _localVariableTable.Insert(_returnBufferArgIndex.Value, returnBuffer);

                _parameterCount++;
            }
        }

        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var method = methodCodeNodeNeedingCode.Method;
            _parameterCount = method.Parameters.Count;

            SetupLocalVariableTable(method);

            if (!method.IsIntrinsic() && !method.IsPinvokeImpl)
            {
                var ilImporter = _ilImporterFactory.Create();
                var flowgraph = new Flowgraph();
                var codeGenerator = _codeGeneratorFactory.Create();

                // Main phases of the compiler live here
                var basicBlocks = ilImporter.Import(_parameterCount, _returnBufferArgIndex, method, _localVariableTable);

                if (_configuration.DumpIRTrees)
                {
                    _logger.LogInformation("METHOD: {methodFullName}", method.FullName);

                    int lclNum = 0;
                    StringBuilder sb = new StringBuilder();
                    foreach (var lclVar in _localVariableTable)
                    {
                        sb.AppendLine($"LCLVAR {lclNum} {lclVar.Name} {lclVar.IsParameter} {lclVar.Kind}");

                        lclNum++;
                    }
                    _logger.LogInformation("{localVars}", sb.ToString());

                    var treeDumper = new TreeDumper();
                    var treedump = treeDumper.Dump(basicBlocks);
                    _logger.LogInformation("{treedump}", treedump);
                }

                flowgraph.SetBlockOrder(basicBlocks);

                var ssaBuilder = new SsaBuilder();
                ssaBuilder.Build(basicBlocks);

                if (_configuration.DumpIRTrees)
                {
                    // Dump LIR here
                    var lirDumper = new LIRDumper();
                    var lirDump = lirDumper.Dump(basicBlocks);
                    _logger.LogInformation("{lirDump}", lirDump);
                }

                var instructions = codeGenerator.Generate(basicBlocks, _localVariableTable, methodCodeNodeNeedingCode);
                methodCodeNodeNeedingCode.MethodCode = instructions;
            }
        }
    }
}
