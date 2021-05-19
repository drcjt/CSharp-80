using System;
using System.Reflection;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;
using dnlib.DotNet;
using System.IO;
using dnlib.DotNet.Emit;

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

                Console.WriteLine("ORG 4A00H ;Start at location 4A00H");

                var entryPointMethodBody = module.EntryPoint.MethodBody as CilBody;
                foreach (var instruction in entryPointMethodBody.Instructions)
                {
                    var code = instruction.OpCode.Code;
                    switch (code)
                    {
                        case Code.Ldc_I4_S:
                            Console.WriteLine($"LD HL, {instruction.Operand} ; Ldc_I4_S");
                            Console.WriteLine("PUSH HL");
                            break;

                        case Code.Add:
                            Console.WriteLine("POP HL");
                            Console.WriteLine("POP DE");
                            Console.WriteLine("ADD HL, DE");
                            Console.WriteLine("PUSH HL");
                            break;

                        case Code.Stloc_0:
                            break;

                        case Code.Stloc_1:
                            break;

                        case Code.Ldloc_0:
                            break;

                        case Code.Ldloc_1:
                            break;

                        case Code.Ret:
                            // Convert to Z80 RET instruction
                            Console.WriteLine("RET ; Ret");
                            break;

                        default:
                            throw new Exception($"Cannot translate IL opcode {code}");
                    }
                }

                Console.WriteLine("END 4A00 ;End-Start of 4A00H");
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
