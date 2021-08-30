using ILCompiler.Compiler;
using ILCompiler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace ILCompiler
{
    public class Program
    {
        private FileInfo _outputFilePath;
        private FileInfo _inputFilePath;
        private static IConfiguration _configuration;

        public static int Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                using (ServiceProvider serviceProvider = services.BuildServiceProvider())
                {
                    Program app = serviceProvider.GetService<Program>();
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

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole()).AddTransient<Program>();
            services.AddSingleton<ICompilation, Compilation>();
            services.AddSingleton(sp => _configuration);
            services.AddSingleton<INameMangler, NameMangler>();
        }

        private void Run(string[] args, ServiceProvider serviceProvider)
        {
            var result = ParseCommandLine(args);

            if (result == 0)
            {
                var compiler = serviceProvider.GetService<ICompilation>();
                compiler.Compile(_inputFilePath.FullName, _outputFilePath.FullName);
            }
            else
            {
                _logger.LogError("Failed to parse command line");
            }
        }

        private int ParseCommandLine(string[] args)
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
                new Argument<FileInfo>("inputFilePath"),
            };

            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";
            rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo, Configuration>(HandleCommand);

            return rootCommand.InvokeAsync(args).Result;
        }

        private void HandleCommand(FileInfo inputFilePath, FileInfo outputFile, Configuration configuration)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFile;
            _configuration = configuration;
        }
    }
}
