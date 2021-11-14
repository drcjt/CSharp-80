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
        private readonly MethodCompilerFactory _methodCompilerFactory;
        private readonly Z80Writer _z80Writer;
        private readonly TypeDependencyAnalyser _typeDependencyAnalyser;

        public Compilation(IConfiguration configuration, ILogger<Compilation> logger, MethodCompilerFactory methodCompilerFactory, Z80Writer z80Writer, TypeDependencyAnalyser typeDependencyAnalyser)
        {
            _configuration = configuration;
            _logger = logger;
            _methodCompilerFactory = methodCompilerFactory;
            _z80Writer = z80Writer;
            _typeDependencyAnalyser = typeDependencyAnalyser;
        }

        public void Compile(string inputFilePath, string outputFilePath)
        {
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

            var rootNode = _typeDependencyAnalyser.AnalyseDependencies(typesToCompile, module.EntryPoint);

            CompileNode(rootNode);

            _z80Writer.OutputCode(rootNode, inputFilePath, outputFilePath);
        }

        private void CompileNode(Z80MethodCodeNode node)
        {
            if (!node.Compiled)
            {
                _logger.LogDebug("Compiling method {method.Name}", node.Method.Name);

                var methodCompiler = _methodCompilerFactory.GetMethodCompiler();
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