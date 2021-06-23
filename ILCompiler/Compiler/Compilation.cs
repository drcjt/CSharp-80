using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.z80.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

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
            string corlibFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), "cs80corlib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corlibFilePath, modCtx);

            var typesToCompile = new List<TypeDef>();
            typesToCompile.AddRange(corlibModule.Types);
            typesToCompile.AddRange(module.Types);

            var dependencyAnalyser = new TypeDependencyAnalyser(this);
            var rootNode = dependencyAnalyser.AnalyseDependencies(typesToCompile, module.EntryPoint);

            CompileNode(rootNode);

            z80Writer.OutputCode(rootNode);
        }

        private void CompileNode(Z80MethodCodeNode node)
        {
            if (!node.Compiled)
            {
                _logger.LogInformation("Compiling method {method.Name}", node.Method.Name);

                var methodCompiler = new MethodCompiler(this);
                methodCompiler.CompileMethod(node);
                node.Compiled = true;

                foreach (var dependentNode in node.Dependencies)
                {
                    CompileNode(dependentNode);
                }
            }
        }
    }
}
