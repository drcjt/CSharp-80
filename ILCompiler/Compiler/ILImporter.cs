using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public partial class ILImporter
    {
        private readonly Compilation _compilation;
        private readonly MethodDef _method;

        private BasicBlock[] _basicBlocks;
        private BasicBlock _currentBasicBlock;
        private BasicBlock _pendingBasicBlocks;

        public INameMangler NameMangler => _compilation.NameMangler;

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        public ILImporter(Compilation compilation, MethodDef method)
        {
            _compilation = compilation;
            _method = method;
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

        private void ImportAppendTree(StackEntry entry)
        {
            _currentBasicBlock.Statements.Add(entry);
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock block)
        {
            _currentBasicBlock = block;
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
                        ImportLoadInt(opcode - Code.Ldc_I4_0, StackValueKind.Int16);
                        break;

                    case Code.Ldc_I4:
                        {
                            var value = (int)currentInstruction.Operand;
                            if (value < Int16.MinValue || value > Int16.MaxValue)
                            {
                                throw new NotSupportedException("Cannot load I4 that is not an I2");
                            }
                            ImportLoadInt((int)currentInstruction.Operand, StackValueKind.Int16);
                        }
                        break;

                    case Code.Ldc_I4_S:
                        ImportLoadInt((sbyte)currentInstruction.Operand, StackValueKind.Int16);
                        break;

                    case Code.Stloc_0:
                    case Code.Stloc_1:
                    case Code.Stloc_2:
                    case Code.Stloc_3:
                        ImportStoreVar(opcode - Code.Stloc_0, false);
                        break;

                    case Code.Ldloc_0:
                    case Code.Ldloc_1:
                    case Code.Ldloc_2:
                    case Code.Ldloc_3:
                        ImportLoadVar(opcode - Code.Ldloc_0, false);
                        break;

                    case Code.Stind_I1:
                        ImportStoreIndirect(WellKnownType.SByte);
                        break;

                    case Code.Add:
                    case Code.Sub:
                    case Code.Mul:
                        ImportBinaryOperation(opcode);
                        break;

                    case Code.Br_S:
                    case Code.Blt_S:
                    case Code.Bgt_S:
                    case Code.Ble_S:
                    case Code.Bge_S:
                    case Code.Beq_S:
                    case Code.Bne_Un_S:
                    case Code.Brfalse_S:
                    case Code.Brtrue_S:
                        {
                            var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                            ImportBranch(opcode + (Code.Br - Code.Br_S), _basicBlocks[(int)target.Offset], (opcode != Code.Br) ? _basicBlocks[currentOffset] : null);
                        }
                        return;

                    case Code.Br:
                    case Code.Blt:
                    case Code.Bgt:
                    case Code.Ble:
                    case Code.Bge:
                    case Code.Beq:
                    case Code.Bne_Un:
                    case Code.Brfalse:
                    case Code.Brtrue:
                        {
                            var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                            ImportBranch(opcode, _basicBlocks[(int)target.Offset], (opcode != Code.Br) ? _basicBlocks[currentOffset] : null);
                        }
                        return;

                    case Code.Ldarg_0:
                    case Code.Ldarg_1:
                    case Code.Ldarg_2:
                    case Code.Ldarg_3:
                        ImportLdArg(opcode - Code.Ldarg_0);
                        break;

                    case Code.Ldstr:
                        ImportLoadString(currentInstruction.Operand as string);
                        break;

                    case Code.Conv_I2:
                        break;

                    case Code.Ret:
                        ImportRet(_method);
                        return;

                    case Code.Call:
                        var methodDefOrRef = currentInstruction.Operand as IMethodDefOrRef;
                        var methodDef = methodDefOrRef.ResolveMethodDefThrow();
                        ImportCall(methodDef);
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
                    ImportFallThrough(nextBasicBlock);
                    return;
                }
            }
        }

        private void MarkBasicBlock(BasicBlock basicBlock)
        {
            if (!basicBlock.Marked)
            {
                basicBlock.Next = _pendingBasicBlocks;
                _pendingBasicBlocks = basicBlock;
                basicBlock.Marked = true;
            }
        }

        private void PushExpression(StackEntry expression)
        {
            _stack.Push(expression);
        }
    }
}
