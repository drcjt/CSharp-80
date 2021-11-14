﻿using dnlib.DotNet;
using ILCompiler.Compiler;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    public interface IILImporter
    {
        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDef method, IList<LocalVariableDescriptor> localVariableTable);
    }
}
