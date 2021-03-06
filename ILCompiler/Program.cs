using ILCompiler.Compiler;
using ILCompiler.Interfaces;
using ILCompiler.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;

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
                using (ServiceProvider? serviceProvider = (ServiceProvider)ServiceProviderFactory.ServiceProvider)
                {
                    Program app = serviceProvider.GetRequiredService<Program>();
                    app.Run(args, serviceProvider);
                }
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
                var compiler = serviceProvider.GetRequiredService<ICompilation>();
                compiler.Compile(_inputFilePath.FullName, _outputFilePath.FullName);
            }
            else
            {
                throw new Exception("Failed to parse command line");
            }
        }

        private int ParseCommandLine(string[] args, IConfiguration configuration)
        {
            var rootCommand = new RootCommand
            {
                new Option<FileInfo>(new[] { "-o", "--outputFile" }, "Output file path") { Required = true },
                new Option<bool>(new[] { "-f", "--ignoreUnknownCil" }, "Ignore unknown cil"),
                new Option<bool>(new[] { "-i", "--dontInlineRuntime" }, "Don't inline runtime assembly" ),
                new Option<bool>(new[] { "-r", "--printReturnCode" }, "Print return code" ),
                new Option<string>(new[] { "-cl", "--corelibPath" }, "Core lib path"),
                new Option<bool>(new[] { "-it", "--integrationTests" }, "Compile for integration tests" ),
                new Option<bool>(new[] { "-d", "--dumpIRTrees" }, "Dump IR trees"),
                new Option<TargetArchitecture>(new[] { "-a", "--targetArchitecture" }, "Target Architecture"),
                new Option<int>(new[] { "-ss", "--stackStart" }, "Stack Start Address"),
                new Argument<FileInfo>("inputFilePath"),
            };

            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";
            rootCommand.Handler = CommandHandler.Create<FileInfo?, FileInfo?, Configuration>(
                (inputFilePath, outputFile, parsedConfiguration) =>
                {
                    _inputFilePath = inputFilePath;
                    _outputFilePath = outputFile;
                    configuration.CorelibPath = parsedConfiguration.CorelibPath;
                    configuration.DumpIRTrees = parsedConfiguration.DumpIRTrees;
                    configuration.IgnoreUnknownCil = parsedConfiguration.IgnoreUnknownCil;
                    configuration.PrintReturnCode = parsedConfiguration.PrintReturnCode;
                    configuration.IntegrationTests = parsedConfiguration.IntegrationTests;
                    configuration.DontInlineRuntime = parsedConfiguration.DontInlineRuntime;
                    configuration.TargetArchitecture = parsedConfiguration.TargetArchitecture;
                    configuration.StackStart = parsedConfiguration.StackStart;
                }
            );

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
