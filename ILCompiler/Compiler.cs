using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler
{
    public class Compiler
    {
        private readonly Assembly _assembly;
        private readonly RomRoutines _romRoutines;

        public Compiler(Assembly assembly, RomRoutines romRoutines)
        {
            _assembly = assembly;
            _romRoutines = romRoutines;
        }

        public void CompileMethod(string methodName, CilBody body)
        {
            _assembly.Label(methodName);

            foreach (var instruction in body.Instructions)
            {
                var code = instruction.OpCode.Code;
                switch (code)
                {
                    case Code.Ldc_I4_S:
                        _assembly.Ld(R16.HL, (sbyte)instruction.Operand);
                        _assembly.Push(R16.HL);
                        break;

                    case Code.Add:
                        _assembly.Pop(R16.HL);                        
                        _assembly.Pop(R16.DE);
                        _assembly.Add(R16.HL, R16.DE);
                        _assembly.Push(R16.HL);
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
                        _assembly.Ret();
                        break;

                    case Code.Call:
                        var methoDef = instruction.Operand as MethodDef;
                        if (methoDef.DeclaringType.FullName.StartsWith("System.Console"))
                        {
                            switch (methoDef.Name)
                            {
                                case "Write":
                                    _romRoutines.Display();
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
        }
    }
}
