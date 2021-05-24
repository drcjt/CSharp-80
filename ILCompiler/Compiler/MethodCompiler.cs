using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class MethodCompiler : IMethodCompiler
    {
        private readonly IZ80Assembly _assembly;
        private readonly IRomRoutines _romRoutines;
        private readonly ILogger<MethodCompiler> _logger;
        private readonly IConfiguration _configuration;

        public MethodCompiler(IZ80Assembly assembly, IRomRoutines romRoutines, IConfiguration configuration, ILogger<MethodCompiler> logger)
        {
            _assembly = assembly;
            _romRoutines = romRoutines;
            _logger = logger;
            _configuration = configuration;
        }

        private void CreateStackFrame()
        {
            // Save IX
            _assembly.Push(I16.IX);

            // Use IX as frame pointer
            _assembly.Ld(I16.IX, 0);
            _assembly.Add(I16.IX, R16.SP);
        }

        private void CompileMethodInstructions(CilBody body, short stackFrameSize = 0)
        {
            foreach (var instruction in body.Instructions)
            {
                var code = instruction.OpCode.Code;
                switch (code)
                {
                    case Code.Ldc_I4_0:
                    case Code.Ldc_I4_1:
                    case Code.Ldc_I4_2:
                    case Code.Ldc_I4_3:
                    case Code.Ldc_I4_4:
                    case Code.Ldc_I4_5:
                    case Code.Ldc_I4_6:
                    case Code.Ldc_I4_7:
                    case Code.Ldc_I4_8:
                        _assembly.Ld(R16.HL, (short)instruction.GetLdcI4Value());
                        _assembly.Push(R16.HL);
                        break;

                    case Code.Ldc_I4_S:
                        _assembly.Ld(R16.HL, (sbyte)instruction.Operand);
                        _assembly.Push(R16.HL);
                        break;

                    case Code.Add:
                        _assembly.Pop(R16.HL);
                        _assembly.Pop(R16.DE);
                        _assembly.Add(R16.HL, R16.DE);
                        _assembly.Push(R16.HL);
                        break;

                    case Code.Sub:
                        _assembly.Pop(R16.HL);
                        _assembly.Pop(R16.DE);
                        _assembly.Sbc(R16.HL, R16.DE);
                        _assembly.Push(R16.HL);
                        break;

                    case Code.Ldarg_0:
                        var argumentOffset = stackFrameSize;
                        argumentOffset += 2; // accounts for return address
                        _assembly.Ld(R8.H, I16.IX, (short)(argumentOffset + 1));
                        _assembly.Ld(R8.L, I16.IX, argumentOffset);
                        _assembly.Push(R16.HL);
                        break;

                    case Code.Stloc_0:
                        break;

                    case Code.Stloc_1:
                        break;

                    case Code.Ldloc_0:
                        break;

                    case Code.Ldloc_1:
                        break;

                    case Code.Ret:
                        // Compile code to unwind stack frame here
                        if (stackFrameSize > 0)
                        {
                            _assembly.Pop(I16.IX);
                        }

                        // Swap return value and return address
                        _assembly.Pop(R16.BC);
                        _assembly.Pop(R16.HL);
                        _assembly.Push(R16.BC);
                        _assembly.Push(R16.HL);

                        _assembly.Ret();
                        break;

                    case Code.Call:
                        var methodDef = instruction.Operand as MethodDef;
                        if (methodDef.DeclaringType.FullName.StartsWith("System.Console"))
                        {
                            switch (methodDef.Name)
                            {
                                case "Write":
                                    _romRoutines.Display();
                                    break;
                            }
                        }
                        else
                        {
                            var targetMethod = methodDef.Name;
                            _assembly.Call(targetMethod);
                        }

                        break;

                    default:
                        if (_configuration.IgnoreUnknownCil)
                        {
                            _logger.LogWarning("Unsupported IL opcode {code}", code);
                        }
                        else
                        {
                            throw new UnknownCilException($"Cannot translate IL opcode {code}");
                        }
                        break;
                }
            }
        }

        public void CompileMethod(MethodDef method)
        {
            var methodName = method.Name;
            var body = method.MethodBody as CilBody;

            _assembly.Label(methodName);

            short frameSize = 0;
            if (method.Parameters.Count > 0)
            {
                foreach (var parameter in method.Parameters)
                {
                    var type = parameter.Type;
                    if (type.IsCorLibType)
                    {
                        switch (type.TypeName)
                        {
                            case "Int16":
                                frameSize += 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (frameSize > 0)
                {
                    CreateStackFrame();
                }
            }

            CompileMethodInstructions(body, frameSize);
        }
    }
}
