﻿using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IConfiguration
    {
        public bool DumpIRTrees { get; set; }
        public bool DumpSsa { get; set; }
        public bool DumpFlowGraphs {  get; set; }
        public bool IgnoreUnknownCil { get; set; }
        public bool PrintReturnCode { get; set; }
        public string CorelibPath { get; set; }
        public bool IntegrationTests { get; set; }
        public TargetArchitecture TargetArchitecture { get; set; }
        public int? StackStart { get; set; }
        public string AssemblerArguments { get; set; }
        public string AssemblerOutput { get; set; }
        public bool NoListFile { get; set; }
        public bool SkipArrayBoundsCheck { get; set; }
        public bool SkipNullReferenceCheck { get; set; }
    }
}
