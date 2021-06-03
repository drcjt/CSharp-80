using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Common.TypeSystem.IL
{
    public enum StackValueKind
    {
        Unknown,
        Int16,
        Int32,
        Int64,
        NativeInt,
        Float,
        ByRef,
        ObjRef,
        ValueType
    }
}
