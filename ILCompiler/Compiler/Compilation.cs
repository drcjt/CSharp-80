﻿using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.IoC;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    public class Compilation : ICompilation
    {
        private readonly ILogger<Compilation> _logger;
        private readonly IConfiguration _configuration;
        private readonly Factory<IMethodCompiler> _methodCompilerFactory;
        private readonly Z80Writer _z80Writer;
        private readonly TypeDependencyAnalyser _typeDependencyAnalyser;

        public Compilation(IConfiguration configuration, ILogger<Compilation> logger, Factory<IMethodCompiler> methodCompilerFactory, Z80Writer z80Writer, TypeDependencyAnalyser typeDependencyAnalyser)
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
            ((AssemblyResolver)modCtx.AssemblyResolver).AddToCache(corlibModule);

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibModule.Assembly.ToAssemblyRef()
            };
            ModuleDefMD module = ModuleDefMD.Load(inputFilePath, options);

            var rootNode = _typeDependencyAnalyser.AnalyseDependencies(module.EntryPoint);

            // Write dgml version of dependency graph
            WriteDependencyLog(Path.ChangeExtension(inputFilePath, ".dgml"), rootNode);

            CompileNode(rootNode);

            _z80Writer.OutputCode(rootNode, inputFilePath, outputFilePath);
        }

        private void WriteDependencyLog(string fileName, IDependencyNode root)
        {
            using (FileStream dgmlOutput = new FileStream(fileName, FileMode.Create))
            {
                DgmlWriter.WriteDependencyGraphToStream(dgmlOutput, root);
                dgmlOutput.Flush();
            }
        }

        private void CompileNode(Z80MethodCodeNode node)
        {
            if (!node.Compiled)
            {
                _logger.LogDebug("Compiling method {method.Name}", node.Method.Name);

                var methodCompiler = _methodCompilerFactory.Create();
                methodCompiler.CompileMethod(node);
                node.Compiled = true;

                if (node.Dependencies != null)
                {
                    foreach (var dependentNode in node.Dependencies)
                    {
                        if (dependentNode is Z80MethodCodeNode z80MethodNode)
                        {
                            CompileNode(z80MethodNode);
                        }
                    }
                }
            }
        }
    }
}