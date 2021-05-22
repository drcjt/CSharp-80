using System;
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

                        CompileMethod(method.Name, method.MethodBody as CilBody);
                    }
                }
            }
        }

        public void CompilePrelude(string entryMethodName)
        {
            _assembly.Call(entryMethodName);
            _assembly.Ret();
        }

        public void CompileMethod(string methodName, CilBody body)
        {
            _assembly.Label(methodName);

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

                    case Code.Stloc_0:
                        break;

                    case Code.Stloc_1:
                        break;

                    case Code.Ldloc_0:
                        break;

                    case Code.Ldloc_1:
                        break;

                    case Code.Ret:
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
    }
}
