using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.z80;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    // TODO: This shouldn't really have any dependencies on dnlib/IL stuff
    public class CodeGenerator : IStackEntryVisitor
    {
        private readonly Compilation _compilation;
        private readonly Z80MethodCodeNode _methodCodeNode;

        private readonly Dictionary<string, string> _labelsToStringData = new Dictionary<string, string>();

        public CodeGenerator(Compilation compilation, Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            _compilation = compilation;
            _methodCodeNode = methodCodeNodeNeedingCode;
        }

        public void Generate(IList<BasicBlock> blocks)
        {
#if NEW_CODEGEN
            Append(new LabelInstruction(_compilation.NameMangler.GetMangledMethodName(_methodCodeNode.Method)));

            GenerateStringMap(blocks);

            GenerateProlog();

            foreach (var block in blocks)
            {
                Append(new LabelInstruction(block.Label));

                foreach (var statement in block.Statements)
                {
                    statement.Accept(this);
                }
            }
#endif
        }

        // TODO: Consider making this a separate phase
        private void GenerateStringMap(IList<BasicBlock> blocks)
        {
            // Process all stack entrys and extract string definitions to populate the string map
            foreach (var block in blocks)
            {
                foreach (var statement in block.Statements)
                {
                    if (statement is StringConstantEntry)
                    {
                        var stringConstantEntry = statement as StringConstantEntry;

                        var label = LabelGenerator.GetLabel(LabelType.String);
                        _labelsToStringData[label] = stringConstantEntry.Value;

                        stringConstantEntry.Label = label;
                    }
                }
            }
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
            else if (entry.Kind == StackValueKind.ObjRef)
            {
                // Currently obj refs can only be strings
                Append(Instruction.Ld(R16.HL, (entry as StringConstantEntry).Label));
                Append(Instruction.Push(R16.HL));
            }
        }

        public void Visit(StoreIndEntry entry)
        {
            Append(Instruction.Pop(R16.BC));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.LdInd(R16.HL, R8.C));
        }

        public void Visit(JumpTrueEntry entry)
        {
            Append(Instruction.Jp(Condition.C, entry.TargetLabel));
        }
        
        public void Visit(JumpEntry entry)
        {
            Append(Instruction.Jp(entry.TargetLabel));
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
                    GenerateAdd();
                    break;

                case BinaryOp.SUB:
                    GenerateSub();
                    break;

                case BinaryOp.MUL:
                    GenerateMul();
                    break;

                default:
                    GenerateComparison(entry.Op);
                    break;
            }
        }

        private readonly string[] comparisonRoutinesByOpcode = new string[]
        {
            "EQL",              // Beq
            "GREATERTHANEQ",    // Bge
            "GREATERTHAN",      // Bgt
            "LESSTHANEQ",       // Ble
            "LESSTHAN",         // Blt
            "NOTEQ"             // Bne
        };

        private void GenerateComparison(BinaryOp op)
        {
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Pop(R16.DE));

            Append(Instruction.Call(comparisonRoutinesByOpcode[op - BinaryOp.EQ]));
        }

        private void GenerateAdd()
        {
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Add(R16.HL, R16.DE));
            Append(Instruction.Push(R16.HL));
        }

        private void GenerateSub()
        {
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Sbc(R16.HL, R16.DE));
            Append(Instruction.Push(R16.HL));
        }

        private void GenerateMul()
        {
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.BC));
            Append(Instruction.Call("MUL16"));
            Append(Instruction.Push(R16.HL));
        }

        public void Visit(LocalVariableEntry entry)
        {
            var offset = entry.LocalNumber;
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            if (entry.LocalNumber >= _methodCodeNode.Method.Parameters.Count)
            {
                // Loading a local variable
                var offset = (entry.LocalNumber - _methodCodeNode.Method.Parameters.Count) * 2; // TODO: This needs to take into account differing sizes of local vars

                Append(Instruction.Pop(R16.HL));
                Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
                Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
            }
            else
            {
                // Loading an argument
                var offset = entry.LocalNumber * 2; // TODO: This needs to take into account differing sizes of parameters

                Append(Instruction.Ld(R8.H, I16.IY, (short)-(offset + 1)));
                Append(Instruction.Ld(R8.L, I16.IY, (short)-(offset + 2)));
                Append(Instruction.Push(R16.HL));
            }
        }

        public void Visit(CallEntry entry)
        {
            Append(Instruction.Call(entry.TargetMethod));
        }

        public void Visit(IntrinsicEntry entry)
        {
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteString":
                    Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                    Append(Instruction.Call("PRINT"));
                    break;

                case "WriteInt16":
                    Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                    Append(Instruction.Call("NUM2DEC"));
                    break;

                case "WriteChar":
                    Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                    Append(Instruction.Ld(R8.A, R8.L)); // Load low byte of argument 1 into A
                    Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                    break;
            }
        }

        public void Append(Instruction instruction)
        {
            _methodCodeNode.MethodCode.Add(instruction);
        }
    }
}
