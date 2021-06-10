using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public class ILImporter : IILImporter
    {
        private readonly Compilation _compilation;
        private readonly MethodDef _method;

        private BasicBlock[] _basicBlocks;
        private BasicBlock _pendingBasicBlocks;

        // TODO: Is this needed anymore given that each basic block has it's own evaluation stack??
        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        public ILImporter(Compilation compilation, MethodDef method)
        {
            _compilation = compilation;
            _method = method;
        }

        public void Compile(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var basicBlockAnalyser = new BasicBlockAnalyser(_method, this);
            var offsetToIndexMap = basicBlockAnalyser.FindBasicBlocks();
            _basicBlocks = basicBlockAnalyser.BasicBlocks;

            ImportBasicBlocks(offsetToIndexMap);  // This converts IL to Z80

            List<Instruction> instructions = new();

            // Generate prolog to setup locals if we have any
            var localsCount = methodCodeNodeNeedingCode.Method.Body.Variables.Count;
            if (localsCount > 0)
            {
                GenerateProlog(instructions, (short)localsCount);
            }

            // Loop thru basic blocks here to generate overall code for whole method
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                var basicBlock = _basicBlocks[i];
                if (basicBlock != null)
                {
                    instructions.Add(new LabelInstruction(basicBlock.Label));
                    instructions.AddRange(basicBlock.Instructions);
                }
            }

            methodCodeNodeNeedingCode.SetCode(instructions);
        }

        private void GenerateProlog(IList<Instruction> instructions, int localsCount)
        {
            // TODO: This assumes all locals are 16 bit in size

            instructions.Add(Instruction.Push(I16.IX));
            instructions.Add(Instruction.Ld(I16.IX, 0));
            instructions.Add(Instruction.Add(I16.IX, R16.SP));

            var localsSize = localsCount * 2;

            instructions.Add(Instruction.Ld(R16.HL, (short)-localsSize));
            instructions.Add(Instruction.Add(R16.HL, R16.SP));
            instructions.Add(Instruction.Ld(R16.SP, R16.HL));
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
            // TODO: add any appropriate code to handle the end of importing a basic block
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock block)
        {
            var currentOffset = block.StartOffset;
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
                        block.ImportLoadInt(opcode - Code.Ldc_I4_0, StackValueKind.Int16);
                        break;

                    case Code.Ldc_I4_S:
                        block.ImportLoadInt((sbyte)currentInstruction.Operand, StackValueKind.Int16);
                        break;

                    case Code.Stloc_0:
                    case Code.Stloc_1:
                    case Code.Stloc_2:
                    case Code.Stloc_3:
                        block.ImportStoreVar(opcode - Code.Stloc_0, false);
                        break;

                    case Code.Ldloc_0:
                    case Code.Ldloc_1:
                    case Code.Ldloc_2:
                    case Code.Ldloc_3:
                        block.ImportLoadVar(opcode - Code.Ldloc_0, false);
                        break;

                    case Code.Add:
                    case Code.Sub:
                        block.ImportBinaryOperation(opcode);
                        break;

                    case Code.Br_S:
                    case Code.Blt_S:
                    case Code.Brfalse_S:
                    case Code.Brtrue_S:
                        {
                            var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                            block.ImportBranch(opcode + (Code.Br - Code.Br_S), _basicBlocks[(int)target.Offset], (opcode != Code.Br) ? _basicBlocks[currentOffset] : null);
                        }
                        return;

                    case Code.Br:
                    case Code.Blt:
                    case Code.Brfalse:
                    case Code.Brtrue:
                        {
                            var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                            block.ImportBranch(opcode, _basicBlocks[(int)target.Offset], (opcode != Code.Br) ? _basicBlocks[currentOffset] : null);
                        }
                        return;

                    case Code.Ldarg_0:
                        block.ImportLdArg(0);
                        break;

                    case Code.Conv_I2:
                        break;

                    case Code.Ret:
                        block.ImportRet(_method);
                        return;

                    case Code.Call:
                        block.ImportCall(currentInstruction.Operand as MethodDef);
                        break;

                    default:
                        if (_compilation.Configuration.IgnoreUnknownCil)
                        {
                            _compilation.Logger.LogWarning($"Unsupported IL opcode {opcode}");
                        }
                        else
                        {
                            throw new UnknownCilException($"Unsupported IL opcode {opcode}");
                        }
                        break;
                }

                if (currentOffset == _basicBlocks.Length)
                {
                    return;
                }

                var nextBasicBlock = _basicBlocks[currentOffset];
                if (nextBasicBlock != null)
                {
                    block.ImportFallThrough(nextBasicBlock);
                    return;
                }
            }
        }

        public void AddToPendingBasicBlocks(BasicBlock block)
        {
            block.Next = _pendingBasicBlocks;
            _pendingBasicBlocks = block;
        }
    }
}
