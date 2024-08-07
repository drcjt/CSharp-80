﻿using ILCompiler.Compiler;
using System.CommandLine;

namespace ILCompiler
{
    internal class ConfigurationOptions
    {
        public readonly Option<bool> IgnoreUnknownCilOption = new(new[] { "-f", "--ignoreUnknownCil" }, "Ignore unknown cil");
        public readonly Option<bool> PrintReturnCodeOption = new(new[] { "-r", "--printReturnCode" }, "Print return code");
        public readonly Option<string> CoreLibPathOption = new(new[] { "-cl", "--corelibPath" }, "Core lib path");
        public readonly Option<bool> IntegrationTestsOption = new(new[] { "-it", "--integrationTests" }, "Compile for integration tests");
        public readonly Option<bool> DumpIRTreesOption = new(new[] { "-dir", "--dumpIRTrees" }, "Dump IR trees");
        public readonly Option<bool> DumpSsaOption = new(new[] { "-dssa", "--dumpSsa" }, "Dump Ssa information");
        public readonly Option<bool> DumpFlowGraphs = new(new[] { "-dfg", "--dumpFlowGraphs" }, "Dump Flow Graphs");

        public readonly Option<TargetArchitecture> TargetArchitectureOption = new(new[] { "-a", "--targetArchitecture" }, getDefaultValue : () => TargetArchitecture.TRS80, "Target Architecture");
        public readonly Option<int?> StackStartOption = new(new[] { "-ss", "--stackStart" }, "Stack Start Address");
        public readonly Option<string> AssemblerArguments = new(new[] { "-aa", "--assemblerArguments" }, "Assembler arguments");
        public readonly Option<string> AssemblerOutput = new(new[] { "-ao", "--assemblerOutput" }, "Assembler output type");
        public readonly Option<bool> NoListFile = new(new[] { "-nl", "--noListFile" }, "No list file");
        public readonly Option<bool> SkipArrayBoundsCheck = new(new[] { "-nb", "--noBoundsCheck" }, "No Array Bounds Check");
        public readonly Option<bool> SkipNullReferenceCheck = new(new[] { "-nn", "--noNullCheck" }, "No Null Reference Check");

        public void AddToCommand(Command command)
        {
            command.AddOption(IgnoreUnknownCilOption);
            command.AddOption(PrintReturnCodeOption);
            command.AddOption(CoreLibPathOption);
            command.AddOption(IntegrationTestsOption);
            command.AddOption(DumpIRTreesOption);
            command.AddOption(TargetArchitectureOption);
            command.AddOption(StackStartOption);
            command.AddOption(AssemblerArguments);
            command.AddOption(AssemblerOutput);
            command.AddOption(NoListFile);
            command.AddOption(SkipArrayBoundsCheck);
            command.AddOption(SkipNullReferenceCheck);
        }
    }
}
