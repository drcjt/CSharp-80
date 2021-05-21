using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.z80;
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

        public static int Main(string[] args)
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
            var result = ParseCommandLine(args);

            if (result == 0)
            {
                ModuleContext modCtx = ModuleDef.CreateModuleContext();
                ModuleDefMD module = ModuleDefMD.Load(_inputFilePath.FullName, modCtx);

                var assembly = new Assembly();
                var romRoutines = new RomRoutines(assembly);

                assembly.Label("START");

                var compiler = new Compiler(assembly, romRoutines);

                compiler.CompileMethod(module.EntryPoint.Name, module.EntryPoint.MethodBody as CilBody);

                assembly.End("START");

                assembly.Write(_outputFilePath.FullName, _inputFilePath.FullName);
            }

            return result;
        }

        private int ParseCommandLine(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<FileInfo>(new[] { "-o", "--outputFile" }, "Output file path") { Required = true },
                new Argument<FileInfo>("inputFilePath"),
            };

            rootCommand.Description = "CSharp-80 compiler from C# IL to Z80 for TRS-80 Machines";
            rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>(HandleCommand);

            return rootCommand.InvokeAsync(args).Result;
        }

        private void HandleCommand(FileInfo inputFilePath, FileInfo outputFile)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFile;
        }
    }
}
