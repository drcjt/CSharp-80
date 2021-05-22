﻿using System;
using Microsoft.Extensions.Logging;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.z80;
using IAssembly = ILCompiler.z80.IAssembly;

namespace ILCompiler
{
    public class Compiler : ICompiler
    {
        private readonly IAssembly _assembly;
        private readonly IRomRoutines _romRoutines;
        private readonly ILogger<Compiler> _logger;

        public bool IgnoreUnknownCil { get; set; } = false;

        public Compiler(IAssembly assembly, IRomRoutines romRoutines, ILogger<Compiler> logger)
        {
            _assembly = assembly;
            _romRoutines = romRoutines;
            _logger = logger;
        }

        public void Compile(string inputFilePath)
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(inputFilePath, modCtx);

            CompilePrelude(module.EntryPoint.Name);

            foreach (var type in module.Types)
            {
                _logger.LogInformation("Compiling Type {type.Name}", type.Name);

                foreach (var method in type.Methods)
                {
                    var isIntrinsic = method.HasCustomAttributes && method.CustomAttributes.IsDefined("System.Runtime.CompilerServices.IntrinsicAttribute");

                    if (!method.IsConstructor && !isIntrinsic)
                    {
                        _logger.LogInformation("Compiling method {method.Name}", method.Name);

                        CompileMethod(method);
                    }
                }
            }
        }

        public void CompilePrelude(string entryMethodName)
        {
            _assembly.Call(entryMethodName);
            _assembly.Ret();
        }

        private void CreateStackFrame(short localsSize)
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
                    case Code.Ldc_I4_2:
                        _assembly.Ld(R16.HL, 0x02);
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

                    case Code.Ldarg_0:
                        _assembly.Ld(R8.H, I16.IX, 5);
                        _assembly.Ld(R8.L, I16.IX, 4);
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
                        if (IgnoreUnknownCil)
                        {
                            _logger.LogWarning("Unsupported IL opcode {code}", code);
                        }
                        else
                        {
                            throw new Exception($"Cannot translate IL opcode {code}");
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
                    CreateStackFrame(frameSize);
                }
            }

            CompileMethodInstructions(body, frameSize);
        }
    }
}
