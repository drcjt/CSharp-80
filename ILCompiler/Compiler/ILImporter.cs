using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public class ILImporter
    {
        private readonly Compilation _compilation;
        private readonly MethodDef _method;

        private BasicBlock[] _basicBlocks;

        private BasicBlock _pendingBasicBlocks;

        private EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        private List<Instruction> _instructions = new();

        public ILImporter(Compilation compilation, MethodDef method)
        {
            _compilation = compilation;
            _method = method;
        }

        public void Compile(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var basicBlockAnalyser = new BasicBlockAnalyser(_method);
            var offsetToIndexMap = basicBlockAnalyser.FindBasicBlocks();    // This finds the basic blocks
            _basicBlocks = basicBlockAnalyser.BasicBlocks;

            ImportBasicBlocks(offsetToIndexMap);  // This converts IL to Z80

            // Loop thru basic blocks here to generate overall code for whole method
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                var basicBlock = _basicBlocks[i];
                if (basicBlock != null)
                {
                    Append(new LabelInstruction($"bb{basicBlock.Id}"));
                    Append(basicBlock.Code);
                }
            }

            methodCodeNodeNeedingCode.SetCode(_instructions);
        }

        private void ImportBasicBlocks(IDictionary<int, int> offsetToIndexMap)
        {
            _pendingBasicBlocks = _basicBlocks[0];
            while (_pendingBasicBlocks != null)
            {
                BasicBlock basicBlock = _pendingBasicBlocks;
                _pendingBasicBlocks = basicBlock.Next;

                StartImportingBasicBlock(basicBlock);
                ImportBasicBlock(offsetToIndexMap, basicBlock);
                EndImportingBasicBlock(basicBlock);
            }
        }

        private void StartImportingBasicBlock(BasicBlock basicBlock)
        {
            _stack.Clear();

            // TODO: push entries from the EntryStack of the basicBlock
        }

        private void EndImportingBasicBlock(BasicBlock basicBlock)
        {
            basicBlock.Code = _instructions;
            _instructions = new List<Instruction>();
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock basicBlock)
        {
            var currentOffset = basicBlock.StartOffset;
            var currentIndex = offsetToIndexMap[currentOffset];

            while (true)
            {
                var currentInstruction = _method.Body.Instructions[currentIndex];
                currentOffset += currentInstruction.GetSize();
                currentIndex++;

                var opcode = currentInstruction.OpCode.Code;

                switch (opcode)
                {
                    case Code.Ldc_I4_0:
                    case Code.Ldc_I4_1:
                    case Code.Ldc_I4_2:
                    case Code.Ldc_I4_3:
                    case Code.Ldc_I4_4:
                    case Code.Ldc_I4_5:
                    case Code.Ldc_I4_6:
                    case Code.Ldc_I4_7:
                    case Code.Ldc_I4_8:
                        ImportLoadInt(opcode - Code.Ldc_I4_0, StackValueKind.Int16);
                        break;

                    case Code.Ldc_I4_S:
                        ImportLoadInt((sbyte)currentInstruction.Operand, StackValueKind.Int16);
                        break;

                    case Code.Add:
                    case Code.Sub:
                        ImportBinaryOperation(opcode);
                        break;

                    case Code.Br:
                    case Code.Blt:
                    case Code.Br_S:
                    case Code.Blt_S:
                        var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                        int delta = (int)target.Offset;
                        ImportBranch(opcode, _basicBlocks[currentOffset + delta], (opcode != Code.Br) ? _basicBlocks[currentOffset] : null);
                        break;

                    case Code.Ldarg_0:
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
                        ImportRet();
                        return;

                    case Code.Call:
                        ImportCall(currentInstruction.Operand as MethodDef);
                        break;

                    default:
                        _compilation.Logger.LogWarning($"Unsupport IL opcode {opcode}");
                        break;
                }

                if (currentOffset == _basicBlocks.Length)
                {
                    return;
                }

                var nextBasicBlock = _basicBlocks[currentOffset];
                if (nextBasicBlock != null)
                {
                    ImportFallThrough(nextBasicBlock);
                    return;
                }
            }
        }

        private void ImportBranch(Code opcode, BasicBlock target, BasicBlock fallthrough)
        {
            if (opcode != Code.Br || opcode != Code.Br_S)
            {
                // Gen code here for condition comparison and if true then jump to target basic block via id
            }
            else
            {
                // Gen code here for jump to target basic block based on id of the basic block
            }

            ImportFallThrough(target);

            if (fallthrough != null)
            {
                ImportFallThrough(fallthrough);
            }
        }

        private void ImportFallThrough(BasicBlock next)
        {
            EvaluationStack<StackEntry> entryStack = next.EntryStack;

            if (entryStack != null)
            {
                // Check the entry stack and the current stack are equivalent,
                // i.e. have same length and elements are identical

                if (entryStack.Length != _stack.Length)
                {
                    throw new InvalidProgramException();
                }

                for (int i = 0; i < entryStack.Length; i++)
                {
                    if (entryStack[i].Kind != _stack[i].Kind)
                    {
                        throw new InvalidProgramException();
                    }

                    // TODO: Should this compare the "Type" of the entries too??
                }
            }
            else
            {
                if (_stack.Length > 0)
                {
                    entryStack = new EvaluationStack<StackEntry>(_stack.Length);

                    /*
                    // TODO: Need to understand why this is required
                    for (int i = 0; i < _stack.Length; i++)
                    {
                        entryStack.Push(NewSpillSlot(_stack[i]));
                    }
                    */
                }
                next.EntryStack = entryStack;
            }

            MarkBasicBlock(next);
        }

        private void MarkBasicBlock(BasicBlock basicBlock)
        {
            if (basicBlock.State == BasicBlock.ImportState.Unmarked)
            {
                basicBlock.Next = _pendingBasicBlocks;
                _pendingBasicBlocks = basicBlock;

                basicBlock.State = BasicBlock.ImportState.IsPending;
            }
        }

        private void ImportBinaryOperation(Code opcode)
        {
            var op1 = _stack.Pop();
            var op2 = _stack.Pop();

            // StackValueKind is carefully ordered to make this work
            StackValueKind kind;
            if (op1.Kind > op2.Kind)
            {
                kind = op1.Kind;
            }
            else
            {
                kind = op2.Kind;
            }

            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Binary operations on types other than short not supported yet");
            }

            PushExpression(kind);

            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Pop(R16.DE));

            switch(opcode)
            {
                case Code.Add:
                    Append(Instruction.Add(R16.HL, R16.DE));
                    break;

                case Code.Sub:
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    break;
            }

            Append(Instruction.Push(R16.HL));
        }

        private void PushExpression(StackValueKind kind)
        {
            _stack.Push(new ExpressionEntry(kind));
        }

        private void ImportLdArg(short stackFrameSize)
        {
            var argumentOffset = stackFrameSize;
            argumentOffset += 2; // accounts for return address
            Append(Instruction.Ld(R8.H, I16.IX, (short)(argumentOffset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, argumentOffset));
            Append(Instruction.Push(R16.HL));
        }

        private void ImportRet()
        {
            if (_method.ReturnType.TypeName != "Void")
            {
                Append(Instruction.Pop(R16.BC));
                Append(Instruction.Pop(R16.HL));
                Append(Instruction.Push(R16.BC));
                Append(Instruction.Push(R16.HL));
            }

            Append(Instruction.Ret());
        }

        private void ImportCall(MethodDef methodToCall)
        {
            if (methodToCall.DeclaringType.FullName.StartsWith("System.Console"))
            {
                switch (methodToCall.Name)
                {
                    case "Write":
                        Append(Instruction.Pop(R16.HL));
                        Append(Instruction.Ld(R8.A, R8.L));
                        Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                        break;
                }
            }
            else
            {
                var targetMethod = methodToCall.Name;
                Append(Instruction.Call(targetMethod));
            }
        }

        private void ImportLoadInt(long value, StackValueKind kind)
        {
            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Loading anything other than Int16 not currently supported");
            }

            _stack.Push(new Int16ConstantEntry(checked((short)value)));

            Append(Instruction.Ld(R16.HL, (short)value));
            Append(Instruction.Push(R16.HL));
        }

        private void Append(Instruction instruction)
        {
            _instructions.Add(instruction);
        }

        private void Append(IList<Instruction> instructions)
        {
            _instructions.AddRange(instructions);
        }

        /*
        private void CreateStackFrame()
        {
            // Save IX
            _assembly.Push(I16.IX);

            // Use IX as frame pointer
            _assembly.Ld(I16.IX, 0);
            _assembly.Add(I16.IX, R16.SP);
        }
        */

        /* OLD CODE TO CALCULATE PARAMETER SIZE
         
                    short frameSize = 0;
            if (_method.Parameters.Count > 0)
            {
                foreach (var parameter in _method.Parameters)
                {
                    var type = parameter.Type;
                    if (type.IsCorLibType)
                    {
                        switch (type.TypeName)
                        {
                            case "Int16":
                                frameSize += 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (frameSize > 0)
                {
                    CreateStackFrame();
                }
            }         
         */
    }
}
