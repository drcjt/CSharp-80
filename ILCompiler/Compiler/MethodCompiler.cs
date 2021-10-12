﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;

namespace ILCompiler.Compiler
{
    public class MethodCompiler
    {
        private IList<LocalVariableDescriptor> _localVariableTable;
        public int? ReturnBufferArgIndex { get; private set; }
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
                ReturnBufferArgIndex = hasThis ? 1 : 0;
                _localVariableTable.Insert(ReturnBufferArgIndex.Value, returnBuffer);

                ParameterCount++;
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
                    _compilation.Logger.LogInformation($"METHOD: {method.FullName}");

                    int lclNum = 0;
                    StringBuilder sb = new StringBuilder();
                    foreach (var lclVar in _localVariableTable)
                    {
                        sb.AppendLine($"LCLVAR {lclNum} {lclVar.Name} {lclVar.IsParameter} {lclVar.Kind}");

                        lclNum++;
                    }
                    _compilation.Logger.LogInformation(sb.ToString());

                    var treeDumper = new TreeDumper();
                    var treedump = treeDumper.Dump(basicBlocks);
                    _compilation.Logger.LogInformation(treedump);
                }

                flowgraph.SetBlockOrder(basicBlocks);
                var instructions = codeGenerator.Generate(basicBlocks);
                methodCodeNodeNeedingCode.MethodCode = instructions;
            }
        }
    }
}
