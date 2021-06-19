﻿using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    // RETURN
    public class ReturnEntry : StackEntry
    {
        public StackEntry Return { get; set; }

        public ReturnEntry() : base(StackValueKind.Unknown)
        {
        }

        public ReturnEntry(StackEntry returnValue) : base(returnValue.Kind)
        {
            Return = returnValue;
        }
    }
}
