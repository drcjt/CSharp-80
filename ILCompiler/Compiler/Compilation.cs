using dnlib.DotNet;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Interfaces;
using System.Diagnostics;
using ILCompiler.Compiler.Ssa;

namespace ILCompiler.Compiler
{
    public class Compilation : ICompilation
    {
        private readonly IConfiguration _configuration;
        private readonly Z80AssemblyWriter _z80AssemblyWriter;
        private readonly DependencyAnalyzer _dependencyAnalyzer;
        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly DnlibModule _module;

        public static bool AnyExceptionHandlers { get; set; } = false;

        public Compilation(IConfiguration configuration, Z80AssemblyWriter z80Writer, CorLibModuleProvider corLibModuleProvider, DependencyAnalyzer dependencyAnalyzer, /*TypeSystemContext typeSystemContext, */ DnlibModule module)
        {
            _configuration = configuration;
            _z80AssemblyWriter = z80Writer;
            _corLibModuleProvider = corLibModuleProvider;
            _dependencyAnalyzer = dependencyAnalyzer;
            _module = module;
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

            _corLibModuleProvider.CorLibModule = corlibModule;

            _module.Context.SetSystemModule(_module);

            var debuggingAttribute = GetDebuggingAttribute(module);
            if (debuggingAttribute != null && debuggingAttribute.IsJITOptimizerDisabled)
            {
                _configuration.Optimize = false;
            }

            var rootNode = (Z80MethodCodeNode)_dependencyAnalyzer.AddRoot(_module.Create(module.EntryPoint));

            // Core Dependency Analysis and code output routine
            var nodes = _dependencyAnalyzer.ComputeMarkedNodes();
            AnyExceptionHandlers = nodes.OfType<Z80MethodCodeNode>().Any(n => n.HasExceptionHandlers);

            _z80AssemblyWriter.WriteCode(rootNode, nodes, inputFilePath, outputFilePath);

            // Write dgml version of dependency graph
            WriteDependencyLog(Path.ChangeExtension(inputFilePath, ".dgml"), rootNode);
        }

        private static void WriteDependencyLog(string fileName, IDependencyNode root)
        {
            using (FileStream dgmlOutput = new FileStream(fileName, FileMode.Create))
            {
                DgmlWriter.WriteDependencyGraphToStream(dgmlOutput, root);
                dgmlOutput.Flush();
            }
        }

        private static DebuggableAttribute? GetDebuggingAttribute(ModuleDef module)
        {
            var assembly = module.Assembly;
            var debuggableAttribute = assembly.CustomAttributes.FirstOrDefault(x => x.TypeFullName == typeof(DebuggableAttribute).FullName);

            if (debuggableAttribute is not null)
            {
                var debuggingModes = (DebuggableAttribute.DebuggingModes)debuggableAttribute.ConstructorArguments[0].Value;
                return new DebuggableAttribute(debuggingModes);
            }

            return null;
        }
    }
}
