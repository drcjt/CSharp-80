using dnlib.DotNet;
using ILCompiler.Interfaces;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using IZ80Assembly = ILCompiler.z80.IZ80Assembly;

namespace ILCompiler.Compiler
{
    public class CilCompiler : ICilCompiler
    {
        private readonly IZ80Assembly _assembly;
        private readonly ILogger<CilCompiler> _logger;
        private readonly IMethodCompiler _methodCompiler;

        public CilCompiler(IZ80Assembly assembly, ILogger<CilCompiler> logger, IMethodCompiler methodCompiler)
        {
            _assembly = assembly;
            _logger = logger;
            _methodCompiler = methodCompiler;
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

                        _methodCompiler.CompileMethod(method);
                    }
                }
            }
        }

        public void CompilePrelude(string entryMethodName)
        {
            _assembly.Call(entryMethodName);
            _assembly.Ret();
        }
    }
}
