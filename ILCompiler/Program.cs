using ILCompiler.z80;
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
        private bool _ignoreUnknownCil;

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
            services.AddSingleton<ICompiler, Compiler>();
            services.AddSingleton<IAssembly, Assembly>();
            services.AddSingleton<IRomRoutines, RomRoutines>();
        }

        private int Run(string[] args, ServiceProvider serviceProvider)
        {
            var result = ParseCommandLine(args);

            if (result == 0)
            {
                var compiler = serviceProvider.GetService<ICompiler>();
                compiler.IgnoreUnknownCil = _ignoreUnknownCil;

                compiler.Compile(_inputFilePath.FullName);

                var assembly = serviceProvider.GetService<IAssembly>();
                assembly.Write(_outputFilePath.FullName, _inputFilePath.FullName);
                _logger.LogDebug($"Written compiled file to {_outputFilePath.FullName}");
            }
            else
            {
                _logger.LogError("Failed to parse command line");
            }

            return result;
        }

        private int ParseCommandLine(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<FileInfo>(new[] { "-o", "--outputFile" }, "Output file path") { Required = true },
                new Option<bool>(new[] { "-f", "--ignoreUnknownCil" }, "Ignore unknown cil"),
                new Argument<FileInfo>("inputFilePath"),
            };

            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";
            rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo, bool>(HandleCommand);

            return rootCommand.InvokeAsync(args).Result;
        }

        private void HandleCommand(FileInfo inputFilePath, FileInfo outputFile, bool ignoreUnknownCil = false)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFile;
            _ignoreUnknownCil = ignoreUnknownCil;
        }
    }
}
