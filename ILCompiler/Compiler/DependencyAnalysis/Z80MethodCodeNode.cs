using dnlib.DotNet;
using ILCompiler.z80;
using System.Collections.Generic;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode
    {
        private readonly MethodDef _method;
        private IList<Instruction> _methodCode;

        public Z80MethodCodeNode(MethodDef method)
        {
            _method = method;
        }

        public void SetCode(IList<Instruction> methodCode)
        {
            _methodCode = methodCode;
        }

        public MethodDef Method
        {
            get
            {
                return _method;
            }
        }

        public IList<Instruction> MethodCode
        {
            get
            {
                return _methodCode;
            }
        }
    }
}
