using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace ILCompiler.Compiler
{
    public class Compilation : ICompilation
    {
        private readonly ILogger<Compilation> _logger;
        private readonly IConfiguration _configuration;
        private readonly INameMangler _nameMangler;

        public ILogger<Compilation> Logger => _logger;
        public INameMangler NameMangler => _nameMangler;

        public Compilation(IConfiguration configuration, ILogger<Compilation> logger, INameMangler nameMangler)
        {
            _configuration = configuration;
            _logger = logger;
            _nameMangler = nameMangler;
        }

        public void Compile(string inputFilePath, string outputFilePath)
        {
            var z80Writer = new Z80Writer(this, inputFilePath, outputFilePath, _configuration);

            ModuleContext modCtx = ModuleDef.CreateModuleContext();

            var corlibFilePath = _configuration.CorelibPath;
            if (string.IsNullOrEmpty(corlibFilePath))
            {
                var inputDirectoryName = Path.GetDirectoryName(inputFilePath);
                if (inputDirectoryName != null)
                {
                    corlibFilePath = Path.Combine(inputDirectoryName, "System.Private.CoreLib.dll");
                }
            }
            ModuleDefMD corlibModule = ModuleDefMD.Load(corlibFilePath, modCtx);
            var corlibAssemblyRef = corlibModule.Assembly.ToAssemblyRef();

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibAssemblyRef
            };

            ModuleDefMD module = ModuleDefMD.Load(inputFilePath, options);

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
                _logger.LogDebug("Compiling method {method.Name}", node.Method.Name);

                var methodCompiler = new MethodCompiler(this, _configuration);
                methodCompiler.CompileMethod(node);
                node.Compiled = true;

                if (node.Dependencies != null)
                {
                    foreach (var dependentNode in node.Dependencies)
                    {
                        CompileNode(dependentNode);
                    }
                }
            }
        }
    }
}