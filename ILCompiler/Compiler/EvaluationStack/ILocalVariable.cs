﻿namespace ILCompiler.Compiler.EvaluationStack
{
    public interface ILocalVariable
    {
        public int LocalNumber { get; }
        public int SsaNumber { get; set; }
    }
}
