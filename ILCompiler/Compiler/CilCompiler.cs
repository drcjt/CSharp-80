using dnlib.DotNet;
using ILCompiler.Interfaces;
using ILCompiler.z80;
using ILCompiler.z80.Interfaces;
using Microsoft.Extensions.Logging;
using IZ80Assembly = ILCompiler.z80.IZ80Assembly;

namespace ILCompiler.Compiler
{
    public class CilCompiler : ICilCompiler
    {
        private readonly IZ80Assembly _assembly;
        private readonly ILogger<CilCompiler> _logger;
        private readonly IOptimizer _optimizer;
        private readonly IMethodCompilerFactory _methodCompilerFactory;

        public CilCompiler(IZ80Assembly assembly, ILogger<CilCompiler> logger, IOptimizer optimizer, IMethodCompilerFactory methodCompilerFactory)
        {
            _assembly = assembly;
            _logger = logger;
            _optimizer = optimizer;
            _methodCompilerFactory = methodCompilerFactory;
        }

        public void Compile(string inputFilePath, string outputFilePath = null)
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(inputFilePath, modCtx);

            GenerateProlog(module.EntryPoint.Name);

            foreach (var type in module.Types)
            {
                _logger.LogInformation("Compiling Type {type.Name}", type.Name);

                foreach (var method in type.Methods)
                {
                    var isIntrinsic = method.HasCustomAttributes && method.CustomAttributes.IsDefined("System.Runtime.CompilerServices.IntrinsicAttribute");

                    if (!method.IsConstructor && !isIntrinsic)
                    {
                        _logger.LogInformation("Compiling method {method.Name}", method.Name);

                        var methodCompiler = _methodCompilerFactory.Create(method);
                        methodCompiler.CompileMethod();
                    }
                }
            }

            GenerateEpilog();

            _optimizer.Optimize(_assembly);

            if (outputFilePath != null)
            {
                _assembly.Write(outputFilePath, inputFilePath);
                _logger.LogDebug($"Written compiled file to {outputFilePath}");
            }
        }

        private void GenerateProlog(string entryMethodName)
        {
            _assembly.Org(0x5200);
            _assembly.Label("START");

            _assembly.Call(entryMethodName);

            _assembly.Pop(R16.BC);  // return value
            _assembly.Pop(R16.HL);  // return address
            _assembly.Push(R16.BC);
            _assembly.Push(R16.HL);
            
            _assembly.Ret();
        }

        private void GenerateEpilog()
        {
            _assembly.End("START");
        }
    }
}
