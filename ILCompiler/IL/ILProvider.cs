﻿using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.IL
{
    public abstract class ILProvider
    {
        public abstract MethodIL? GetMethodIL(MethodDesc method, DnlibModule module);
    }
}
