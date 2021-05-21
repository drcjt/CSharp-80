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

                assembly.LABEL("START");

                var entryPointMethodName = module.EntryPoint.Name;
                var entryPointMethodBody = module.EntryPoint.MethodBody as CilBody;
                
                assembly.LABEL(entryPointMethodName);

                foreach (var instruction in entryPointMethodBody.Instructions)
                {
                    var code = instruction.OpCode.Code;
                    switch (code)
                    {
                        case Code.Ldc_I4_S:
                            assembly.LD(R16.HL, (sbyte)instruction.Operand);
                            assembly.PUSH(R16.HL);
                            break;

                        case Code.Add:
                            assembly.POP(R16.HL);
                            assembly.POP(R16.DE);
                            //assembly.Add(R16.HL, R16.DE);
                            assembly.PUSH(R16.HL);
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
                            assembly.RET();
                            break;

                        case Code.Call:
                            var memberRef = instruction.Operand as MemberRef;
                            if (memberRef.DeclaringType.FullName.StartsWith("System.Console"))
                            {
                                switch (memberRef.Name)
                                {
                                    case "Write":
                                        // Output to current cursor position
                                        assembly.POP(R16.HL);
                                        assembly.LD(R8.A, R8.L);
                                        assembly.CALL(0x0033);  // ROM routine to display character at current cursor position
                                        break;
                                }
                            }
                            else
                            {
                                // TODO: implement proper compilation of CIL Call instruction here
                            }

                            break;

                        default:
                            throw new Exception($"Cannot translate IL opcode {code}");
                    }
                }

                assembly.END("START");

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
