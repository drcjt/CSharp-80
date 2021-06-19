using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler
{
    public class CodeGenerator : IStackEntryVisitor
    {
        private readonly Z80MethodCodeNode _methodCodeNode;
        public CodeGenerator(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            _methodCodeNode = methodCodeNodeNeedingCode;
        }

        public void Generate(IList<BasicBlock> blocks)
        {
#if NEW_CODEGEN
            GenerateProlog();

            foreach (var block in blocks)
            {
                foreach (var statement in block.Statements)
                {
                    statement.Accept(this);
                }
            }
#endif
        }

        private void GenerateProlog()
        {
            // TODO: This assumes all locals are 16 bit in size
            var instructions = _methodCodeNode.MethodCode;

            var paramsCount = _methodCodeNode.Method.Parameters.Count;
            if (paramsCount > 0)
            {
                instructions.Add(Instruction.Push(I16.IY));
                // Set IY to start of arguments here
                // IY = SP - (2 * (number of params + 2))

                instructions.Add(Instruction.Ld(R16.HL, (short)(2 * (paramsCount + 2))));
                instructions.Add(Instruction.Add(R16.HL, R16.SP));
                instructions.Add(Instruction.Push(R16.HL));
                instructions.Add(Instruction.Pop(I16.IY));
            }

            var localsCount = _methodCodeNode.Method.Body.Variables.Count;
            if (localsCount > 0)
            {
                instructions.Add(Instruction.Push(I16.IX));
                instructions.Add(Instruction.Ld(I16.IX, 0));
                instructions.Add(Instruction.Add(I16.IX, R16.SP));

                var localsSize = localsCount * 2;

                instructions.Add(Instruction.Ld(R16.HL, (short)-localsSize));
                instructions.Add(Instruction.Add(R16.HL, R16.SP));
                instructions.Add(Instruction.Ld(R16.SP, R16.HL));
            }
        }

        public void Visit(ConstantEntry entry)
        {
            if (entry.Kind == StackValueKind.Int16)
            {
                var value = (entry as Int16ConstantEntry).Value;
                Append(Instruction.Ld(R16.HL, (short)value));
                Append(Instruction.Push(R16.HL));
            }
            else if (entry.Kind == StackValueKind.Int32)
            {
                var value = (entry as Int32ConstantEntry).Value;
                var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
                var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

                Append(Instruction.Ld(R16.HL, low));
                Append(Instruction.Push(R16.HL));
                Append(Instruction.Ld(R16.HL, high));
                Append(Instruction.Push(R16.HL));
            }
        }

        public void Visit(IndEntry entry)
        {
            throw new NotImplementedException();
        }

        public void Visit(JumpTrueEntry entry)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnEntry entry)
        {
            var method = _methodCodeNode.Method;
            var hasReturnValue = method.HasReturnType;
            var hasParameters = method.Parameters.Count > 0;
            var hasLocals = method.Body.Variables.Count > 0;

            if (hasReturnValue)
            {
                Append(Instruction.Pop(R16.BC));            // Copy return value into BC
            }

            if (hasLocals)
            {
                Append(Instruction.Ld(R16.SP, I16.IX));     // Move SP to before locals
                Append(Instruction.Pop(I16.IX));            // Remove IX
            }

            if (hasParameters)
            {
                Append(Instruction.Pop(R16.BC));            // Remove IY
                Append(Instruction.Pop(R16.HL));            // Store return address in HL
                Append(Instruction.Ld(R16.SP, I16.IY));     // Reset SP to before arguments

                Append(Instruction.Push(R16.BC));           // Restore IY
                Append(Instruction.Pop(I16.IY));
            }

            if (hasReturnValue)
            {
                Append(Instruction.Pop(R16.HL));            // Store return address in HL
                Append(Instruction.Push(R16.BC));           // Push return value
                Append(Instruction.Push(R16.HL));           // Push return address
            }
            else if (hasParameters)
            {
                Append(Instruction.Push(R16.HL));           // Push return address
            }

            Append(Instruction.Ret());
        }

        public void Visit(BinaryOperator entry)
        {
            switch (entry.Op)
            {
                case BinaryOp.ADD:
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Add(R16.HL, R16.DE));
                    break;

                case BinaryOp.SUB:
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    break;

                case BinaryOp.MUL:
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.BC));
                    Append(Instruction.Call("MUL16"));
                    break;
            }
            Append(Instruction.Push(R16.HL));
        }

        public void Visit(LocalVariableEntry entry)
        {
            var offset = entry.LocalNumber;
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
        }

        public void Visit(AssignmentEntry entry)
        {
            throw new NotImplementedException();
        }

        public void Visit(CallEntry entry)
        {
            Append(Instruction.Call(entry.TargetMethod));
        }

        public void Visit(IntrinsicEntry entry)
        {
            throw new NotImplementedException();
        }
        public void Append(Instruction instruction)
        {
            _methodCodeNode.MethodCode.Add(instruction);
        }

    }
}
