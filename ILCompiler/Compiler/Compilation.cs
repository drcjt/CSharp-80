using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.z80.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class Compilation : ICompilation
    {
        private readonly ILogger<Compilation> _logger;
        private readonly IOptimizer _optimizer;
        private readonly IConfiguration _configuration;
        private readonly INameMangler _nameMangler;

        public ILogger<Compilation> Logger => _logger;
        public IConfiguration Configuration => _configuration;
        public IOptimizer Optimizer => _optimizer;
        public INameMangler NameMangler => _nameMangler;

        public Compilation(IConfiguration configuration, ILogger<Compilation> logger, IOptimizer optimizer, INameMangler nameMangler)
        {
            _configuration = configuration;
            _logger = logger;
            _optimizer = optimizer;
            _nameMangler = nameMangler;
        }

        public void Compile(string inputFilePath, string outputFilePath)
        {
            var z80Writer = new Z80Writer(this, inputFilePath, outputFilePath);

            ModuleContext modCtx = ModuleDef.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(inputFilePath, modCtx);
            string corlibFilePath = "cs80corlib.dll";
            ModuleDefMD corlibModule = ModuleDefMD.Load(corlibFilePath, modCtx);

            var typesToCompile = new List<TypeDef>();
            typesToCompile.AddRange(corlibModule.Types);
            typesToCompile.AddRange(module.Types);

            var nodes = new List<Z80MethodCodeNode>();

            foreach (var type in typesToCompile)
            {
                _logger.LogInformation("Compiling Type {type.Name}", type.Name);

                foreach (var method in type.Methods)
                {
                    if (!method.IsConstructor && !method.IsIntrinsic() && !method.IsPinvokeImpl)
                    {
                        _logger.LogInformation("Compiling method {method.Name}", method.Name);

                        var methodCodeNodeNeedingCode = new Z80MethodCodeNode(method);
                        nodes.Add(methodCodeNodeNeedingCode);

                        z80Writer.CompileMethod(methodCodeNodeNeedingCode);
                    }
                }
            }

            z80Writer.OutputCode(nodes, module.EntryPoint);
        }
    }
}
