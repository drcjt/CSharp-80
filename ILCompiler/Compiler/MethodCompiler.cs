﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ILCompiler.Compiler
{
    public class MethodCompiler : IMethodCompiler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MethodCompiler> _logger;
        private readonly IPhaseFactory _phaseFactory;
        private readonly ILProvider _ilProvider;

        private int _parameterCount;
        private int? _returnBufferArgIndex;

        private readonly IList<LocalVariableDescriptor> _localVariableTable;

        public MethodCompiler(ILogger<MethodCompiler> logger, IConfiguration configuration, IPhaseFactory phaseFactory, RTILProvider ilProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _localVariableTable = new List<LocalVariableDescriptor>();
            _phaseFactory = phaseFactory;
            _ilProvider = ilProvider;
        }

        private void SetupLocalVariableTable(MethodDesc method)
        {
            var body = method.Body;

            // Setup local variable table - includes parameters as well as locals in method
            foreach (var parameter in method.Parameters())
            {
                var local = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    IsTemp = false,
                    Name = parameter.Name,
                    ExactSize = parameter.Type.GetInstanceFieldSize(),
                    Type = parameter.Type.GetVarType(),
                };
                _localVariableTable.Add(local);
            }

            if (body != null)
            {
                foreach (var local in method.Locals())
                {
                    var localVariableDescriptor = new LocalVariableDescriptor()
                    {
                        IsParameter = false,
                        IsTemp = false,
                        Name = local.Name,
                        ExactSize = local.Type.GetInstanceFieldSize(),
                        Type = local.Type.GetVarType(),
                    };
                    _localVariableTable.Add(localVariableDescriptor);
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
                var target = new TargetDetails(Common.TypeSystem.Common.TargetArchitecture.Z80);

                var returnBuffer = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    Type = VarType.ByRef,
                    IsTemp = false,
                    ExactSize = target.PointerSize,
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

            if (method.HasCustomAttribute("System.Runtime", "RuntimeImportAttribute"))
            {
                return;
            }
            if (method.IsPInvokeImpl || method.IsInternalCall)
            {
                return;
            }
            if (method.IsIntrinsic)
            {
                var methodIL = _ilProvider.GetMethodIL(method);
                if (methodIL == null)
                {
                    return;
                }

                method.Body = methodIL;
            }

            _parameterCount = method.Parameters().Count;

            SetupLocalVariableTable(method);

            var ilImporter = _phaseFactory.Create<IILImporter>();

            // Main phases of the compiler live here
            var basicBlocks = ilImporter.Import(_parameterCount, _returnBufferArgIndex, method, _localVariableTable);

            if (_configuration.DumpIRTrees)
            {
                _logger.LogInformation("METHOD: {methodFullName}", method.FullName);

                int lclNum = 0;
                StringBuilder sb = new();
                foreach (var lclVar in _localVariableTable)
                {
                    sb.AppendLine($"LCLVAR {lclNum} {lclVar.Name} {lclVar.IsParameter} {lclVar.Type} {lclVar.ExactSize}");

                    lclNum++;
                }
                _logger.LogInformation("{localVars}", sb.ToString());

                var treeDumper = new TreeDumper();
                var treedump = treeDumper.Dump(basicBlocks);
                _logger.LogInformation("{treedump}", treedump);
            }

            var morpher = _phaseFactory.Create<IMorpher>();
            morpher.Morph(basicBlocks);

            var flowgraph = _phaseFactory.Create<IFlowgraph>();
            flowgraph.SetBlockOrder(basicBlocks);

            var ssaBuilder = _phaseFactory.Create<ISsaBuilder>();
            ssaBuilder.Build(basicBlocks, _localVariableTable, _configuration.DumpSsa);

            if (_configuration.DumpIRTrees)
            {
                // Dump LIR here
                var lirDumper = new LIRDumper();
                var lirDump = lirDumper.Dump(basicBlocks);
                _logger.LogInformation("{lirDump}", lirDump);
            }

            // Rationalize
            var rationalizer = _phaseFactory.Create<IRationalizer>();
            rationalizer.Rationalize(basicBlocks);

            // Lower
            var lowering = _phaseFactory.Create<ILowering>();
            lowering.Run(basicBlocks);

            var codeGenerator = _phaseFactory.Create<ICodeGenerator>();
            var instructions = codeGenerator.Generate(basicBlocks, _localVariableTable, methodCodeNodeNeedingCode);
            methodCodeNodeNeedingCode.MethodCode = GetMethodCode(instructions);
        }

        private static string GetMethodCode(IList<ILCompiler.Compiler.Emit.Instruction> instructions)
        {
            var sb = new StringBuilder();
            foreach (var instruction in instructions)
            {
                sb.AppendLine(instruction.ToString());
            }
            return sb.ToString();
        }
    }
}
