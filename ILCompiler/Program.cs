using System;
using System.Reflection;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;

namespace ILCompiler
{
    class Program
    {
        private string _outputFilePath;
        private string _inputFilePath;

        static int Main(string[] args)
        {
            try
            {
                return new Program().Run(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: " + e.Message);
                Console.Error.WriteLine(e.ToString());
                return 1;
            }
        }

        private int Run(string[] args)
        {
            return ParseCommandLine(args);
        }

        private int ParseCommandLine(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Argument<string>("Input file to compile"),
                new Option<string>(new[] { "-o", "--out" }, "Output file path") { Required = true },
            };

            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";
            rootCommand.Handler = CommandHandler.Create<string, string>(HandleCommand);

            return rootCommand.Invoke(args);
        }

        private void HandleCommand(string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;
        }
    }
}
