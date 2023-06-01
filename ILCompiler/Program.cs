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
            var outputFileOption = new Option<FileInfo>(new[] { "-o", "--outputFile" }, "Output file path") { IsRequired = true };
            var inputFileArgument = new Argument<FileInfo>("inputFilePath");

            var configurationOptions = new ConfigurationOptions();
            var configurationBinder = new ConfigurationBinder(configurationOptions);

            var rootCommand = new RootCommand();
            rootCommand.AddArgument(inputFileArgument);
            rootCommand.AddOption(outputFileOption);
            configurationOptions.AddToCommand(rootCommand);
            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";

            rootCommand.SetHandler(
                (FileInfo? inputFilePath, FileInfo? outputFile, IConfiguration parsedConfiguration) => 
                {
                    _inputFilePath = inputFilePath;
                    _outputFilePath = outputFile;
                    configuration.CorelibPath = parsedConfiguration.CorelibPath;
                    configuration.DumpIRTrees = parsedConfiguration.DumpIRTrees;
                    configuration.IgnoreUnknownCil = parsedConfiguration.IgnoreUnknownCil;
                    configuration.PrintReturnCode = parsedConfiguration.PrintReturnCode;
                    configuration.IntegrationTests = parsedConfiguration.IntegrationTests;
                    configuration.TargetArchitecture = parsedConfiguration.TargetArchitecture;
                    configuration.StackStart = parsedConfiguration.StackStart;
                    configuration.AssemblerArguments = parsedConfiguration.AssemblerArguments;
                    configuration.AssemblerOutput = parsedConfiguration.AssemblerOutput;
                    configuration.NoListFile = parsedConfiguration.NoListFile;
                },
                inputFileArgument, outputFileOption, configurationBinder
            );

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
