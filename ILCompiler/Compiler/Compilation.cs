using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Compilation : ICompilation
    {
        private readonly IConfiguration _configuration;
        private readonly Z80AssemblyWriter _z80AssemblyWriter;
        private readonly DependencyAnalyzer _dependencyAnalyzer;
        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly TypeSystemContext _typeSystemContext;

        public static bool AnyExceptionHandlers { get; set; } = false;

        public Compilation(IConfiguration configuration, Z80AssemblyWriter z80Writer, CorLibModuleProvider corLibModuleProvider, DependencyAnalyzer dependencyAnalyzer, TypeSystemContext typeSystemContext)
        {
            _configuration = configuration;
            _z80AssemblyWriter = z80Writer;
            _corLibModuleProvider = corLibModuleProvider;
            _dependencyAnalyzer = dependencyAnalyzer;
            _typeSystemContext = typeSystemContext;
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

            var rootNode = (Z80MethodCodeNode)_dependencyAnalyzer.AddRoot(_typeSystemContext.Create(module.EntryPoint));

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
    }
}