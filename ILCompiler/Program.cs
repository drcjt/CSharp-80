using ILCompiler.Compiler;
using ILCompiler.Compiler.Peephole;
using ILCompiler.Interfaces;
using ILCompiler.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace ILCompiler
{
    public class Program
    {
        private FileInfo? _outputFilePath;
        private FileInfo? _inputFilePath;

        public static int Main(string[] args)
        {
            try
            {
                var serviceProvider = ServiceProviderFactory.CreateServiceProviderFactory();
                Program app = serviceProvider.GetRequiredService<Program>();
                app.Run(args, serviceProvider);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: " + e.Message);
                Console.Error.WriteLine(e.ToString());
                return 1;
            }

            return 0;
        }

        private readonly ILogger<Program> _logger;
        public Program(ILogger<Program> logger)
        {
            _logger = logger;
        }

        private void Run(string[] args, IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var result = ParseCommandLine(args, configuration);

            if (result == 0 && _inputFilePath != null && _outputFilePath != null)
            {
                _logger.LogDebug("Compiling {inputFilePath} to {outputFilePath}", _inputFilePath.FullName, _outputFilePath.FullName);

                var compiler = serviceProvider.GetRequiredService<ICompilation>();
                compiler.Compile(_inputFilePath.FullName, _outputFilePath.FullName);

                var assembler = serviceProvider.GetRequiredService<IZ80Assembler>();
                assembler.Assemble(_outputFilePath.FullName);
            }
            else
            {
                throw new Exception("Failed to parse command line");
            }
        }

        private int ParseCommandLine(string[] args, IConfiguration configuration)
        {
            var outputFileOption = new Option<FileInfo>("OutputFilePath", "-o", "--outputFile") { Required = true };
            var inputFileArgument = new Argument<FileInfo>("inputFilePath");

            var configurationOptions = new ConfigurationOptions();

            var rootCommand = new RootCommand();
            rootCommand.Arguments.Add(inputFileArgument);
            rootCommand.Options.Add(outputFileOption);
            configurationOptions.AddToCommand(rootCommand);
            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";

            rootCommand.SetAction(parseResult =>
            {
                _inputFilePath = parseResult.GetValue(inputFileArgument);
                _outputFilePath = parseResult.GetValue(outputFileOption);
                configuration.CorelibPath = parseResult.GetValue(configurationOptions.CoreLibPathOption) ?? "";
                configuration.DumpIRTrees = parseResult.GetValue(configurationOptions.DumpIRTreesOption);
                configuration.DumpSsa = parseResult.GetValue(configurationOptions.DumpSsaOption);
                configuration.DumpFlowGraphs = parseResult.GetValue(configurationOptions.DumpFlowGraphs);
                configuration.IgnoreUnknownCil = parseResult.GetValue(configurationOptions.IgnoreUnknownCilOption);
                configuration.PrintReturnCode = parseResult.GetValue(configurationOptions.PrintReturnCodeOption);
                configuration.IntegrationTests = parseResult.GetValue(configurationOptions.IntegrationTestsOption);
                configuration.TargetArchitecture = GetTargetArchitecture(parseResult.GetValue(configurationOptions.RuntimeIdentifier), parseResult.GetValue(configurationOptions.TargetArchitectureOption));
                configuration.StackStart = parseResult.GetValue(configurationOptions.StackStartOption);
                configuration.AssemblerArguments = parseResult.GetValue(configurationOptions.AssemblerArguments) ?? "";
                configuration.AssemblerOutput = parseResult.GetValue(configurationOptions.AssemblerOutput) ?? "";
                configuration.NoListFile = parseResult.GetValue(configurationOptions.NoListFile);                
            });

            return rootCommand.Parse(args).Invoke();
        }

        private static TargetArchitecture GetTargetArchitecture(string? runtimeIdentifier, TargetArchitecture defaultArchitecture)
        {
            if (string.IsNullOrEmpty(runtimeIdentifier))
            {
                return defaultArchitecture;
            }
            else if (runtimeIdentifier == "z80-trs80")
            {
                return TargetArchitecture.TRS80;
            }
            else if (runtimeIdentifier == "z80-zxspectrum")
            {
                return TargetArchitecture.ZXSpectrum;
            }
            else if (runtimeIdentifier == "z80-cpm")
            {
                return TargetArchitecture.CPM;
            }
            else
            {
                throw new ArgumentException($"Unsupported runtime identifier {runtimeIdentifier}");
            }
        }
    }
}
