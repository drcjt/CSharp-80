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
    public class ILImporter
    {
        private readonly MethodCompiler _methodCompiler;
        private readonly MethodDef _method;
        private readonly LocalVariableDescriptor[] _localVariableTable;

        private BasicBlock[] _basicBlocks;
        private BasicBlock _currentBasicBlock;
        private BasicBlock _pendingBasicBlocks;

        public INameMangler NameMangler => _methodCompiler.NameMangler;

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        public ILImporter(MethodCompiler methodCompiler, MethodDef method, LocalVariableDescriptor[] localVariableTable)
        {
            _methodCompiler = methodCompiler;
            _method = method;
            _localVariableTable = localVariableTable;
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
                        ImportLoadInt(opcode - Code.Ldc_I4_0, StackValueKind.Int32);
                        break;

                    case Code.Ldc_I4:
                        {
                            var value = (int)currentInstruction.Operand;
                            ImportLoadInt((int)currentInstruction.Operand, StackValueKind.Int32);
                        }
                        break;

                    case Code.Ldc_I4_S:
                        ImportLoadInt((sbyte)currentInstruction.Operand, StackValueKind.Int32);
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
                    case Code.Div:
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
                        ImportConversion(WellKnownType.Int16, false);
                        break;

                    case Code.Conv_U2:
                        ImportConversion(WellKnownType.UInt16, true);
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
                        if (_methodCompiler.Configuration.IgnoreUnknownCil)
                        {
                            _methodCompiler.Logger.LogWarning($"Unsupported IL opcode {opcode}");
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

        public IList<BasicBlock> Import()
        {
            var basicBlockAnalyser = new BasicBlockAnalyser(_method);
            var offsetToIndexMap = basicBlockAnalyser.FindBasicBlocks();
            _basicBlocks = basicBlockAnalyser.BasicBlocks;

            ImportBasicBlocks(offsetToIndexMap);

            var basicBlocks = new List<BasicBlock>();
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                if (_basicBlocks[i] != null)
                {
                    basicBlocks.Add(_basicBlocks[i]);
                }
            }

            return basicBlocks;
        }

        public void ImportConversion(WellKnownType wellKnownType, bool unsigned)
        {
            // TODO: Need to implement conversion properly
            /*
            var op1 = _stack.Pop();
            if (op1.Kind != desiredKind)
            {
                op1 = new CastEntry(desiredKind, unsigned, op1);
            }
            _stack.Push(op1);
            */
        }

        public void ImportBranch(Code opcode, BasicBlock target, BasicBlock fallthrough)
        {
            if (opcode != Code.Br)
            {
                var op2 = _stack.Pop();
                if (op2.Kind != StackValueKind.Int32)
                {
                    throw new NotSupportedException("Boolean comparisonsonly supported using int as underlying type");
                }

                StackEntry op1;
                Operation op;
                if (opcode != Code.Brfalse && opcode != Code.Brtrue)
                {
                    op1 = _stack.Pop();
                    if (op2.Kind != StackValueKind.Int32)
                    {
                        throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
                    }
                    op = Operation.Eq + (opcode - Code.Beq);
                }
                else
                {
                    op1 = new Int32ConstantEntry((short)(opcode == Code.Brfalse ? 0 : 1));
                    op = Operation.Eq;
                }
                op1 = new BinaryOperator(op, op1, op2, StackValueKind.Int32);
                ImportAppendTree(new JumpTrueEntry(target.Label, op1));
            }
            else
            {
                ImportAppendTree(new JumpEntry(target.Label));
            }

            // Fall through handling
            ImportFallThrough(target);

            if (fallthrough != null)
            {
                ImportFallThrough(fallthrough);
            }
        }

        public void ImportFallThrough(BasicBlock next)
        {
            // Evaluation stack in each basic block holds the imported high level tree representation of the IL

            /*
            EvaluationStack<StackEntry> entryStack = next.Stack;

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
                if (Stack.Length > 0)
                {
                    entryStack = new EvaluationStack<StackEntry>(Stack.Length);

                    // TODO: Need to understand why this is required
                    for (int i = 0; i < Stack.Length; i++)
                    {
                        entryStack.Push(NewSpillSlot(Stack[i]));
                    }
                }
                next.Stack = entryStack;                
            }
            */
            MarkBasicBlock(next);
        }

        public void ImportBinaryOperation(Code opcode)
        {
            var op2 = _stack.Pop();
            var op1 = _stack.Pop();

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

            if (kind != StackValueKind.Int16 && kind != StackValueKind.NativeInt && kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Binary operations on types other than short, int32 and nativeint not supported yet");
            }

            Operation binaryOp;
            switch (opcode)
            {
                case Code.Add:
                    binaryOp = Operation.Add;
                    break;

                case Code.Sub:
                    binaryOp = Operation.Sub;
                    break;

                case Code.Mul:
                    binaryOp = Operation.Mul;
                    break;

                case Code.Div:
                    binaryOp = Operation.Div;
                    break;

                default:
                    throw new NotImplementedException();
            }

            var binaryExpr = new BinaryOperator(binaryOp, op1, op2, kind);
            PushExpression(binaryExpr);
        }

        public void ImportStoreIndirect(WellKnownType type)
        {
            var value = _stack.Pop();
            var addr = _stack.Pop();

            if (type != WellKnownType.SByte && addr.Kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException();
            }

            ImportAppendTree(new StoreIndEntry(addr, value));
        }

        public void ImportStoreVar(int index, bool argument)
        {
            var value = _stack.Pop();
            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException("Storing variables other than short, int32 or object refs not supported yet");
            }
            var localNumber = _methodCompiler.ParameterCount + index;
            var node = new StoreLocalVariableEntry(localNumber, value);
            ImportAppendTree(node);
        }

        public void ImportLoadVar(int index, bool argument)
        {
            var localNumber = _methodCompiler.ParameterCount + index;
            var localVariable = _localVariableTable[localNumber];
            var node = new LocalVariableEntry(localNumber, localVariable.Kind);
            PushExpression(node);
        }

        public void ImportLdArg(int index)
        {
            var argument = _localVariableTable[index];
            var node = new LocalVariableEntry(index, argument.Kind);
            PushExpression(node);
        }

        public void ImportLoadInt(long value, StackValueKind kind)
        {
            if (kind == StackValueKind.Int32)
            {
                PushExpression(new Int32ConstantEntry(checked((int)value)));
            }
            else
            {
                throw new NotSupportedException("Loading anything other than Int16 not currently supported");
            }
        }

        public void ImportLoadString(string str)
        {
            PushExpression(new StringConstantEntry(str));
        }

        private bool ImportIntrinsicCall(MethodDef methodToCall, StackEntry[] arguments)
        {
            // Not yet implemented methods with non void return type
            if (methodToCall.HasReturnType)
            {
                throw new NotSupportedException();
            }

            // Map method name to string that code generator will understand
            var targetMethodName = "";
            switch (methodToCall.Name)
            {
                // TODO: Suspect this won't stay as an intrinsic but at least we have the mechanism for instrincs
                case "Write":
                    if (IsTypeName(methodToCall, "System", "Console"))
                    {
                        var argtype = methodToCall.Parameters[0].Type;
                        switch (argtype.FullName)
                        {
                            case "System.String":
                                targetMethodName = "WriteString";
                                break;

                            case "System.Int32":
                                targetMethodName = "WriteInt32";
                                break;

                            case "System.UInt32":
                                targetMethodName = "WriteUInt32";
                                break;

                            case "System.Char":
                                targetMethodName = "WriteChar";
                                break;

                            default:
                                throw new NotSupportedException();
                        }
                    }
                    break;
                default:
                    return false;
            }

            var callNode = new IntrinsicEntry(targetMethodName, arguments, StackValueKind.Unknown);
            ImportAppendTree(callNode);

            return true;
        }

        private static bool IsTypeName(MethodDef method, string typeNamespace, string typeName)
        {
            var metadataType = method.DeclaringType;
            if (metadataType == null)
            {
                return false;
            }
            return metadataType.Namespace == typeNamespace && metadataType.Name == typeName;
        }

        public void ImportCall(MethodDef methodToCall)
        {
            StackEntry[] arguments = new StackEntry[methodToCall.Parameters.Count];
            for (int i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = _stack.Pop();
                arguments[methodToCall.Parameters.Count - i - 1] = argument;
            }

            // Intrinsic calls
            if (methodToCall.IsIntrinsic())
            {
                if (!ImportIntrinsicCall(methodToCall, arguments))
                {
                    throw new NotSupportedException("Unknown intrinsic");
                }
                return;
            }

            string targetMethod = "";
            if (methodToCall.IsPinvokeImpl)
            {
                targetMethod = methodToCall.ImplMap.Name;
            }
            else
            {
                targetMethod = _methodCompiler.NameMangler.GetMangledMethodName(methodToCall);
            }
            var returnType = methodToCall.ReturnType.GetStackValueKind();
            var callNode = new CallEntry(targetMethod, arguments, returnType);
            if (!methodToCall.HasReturnType)
            {
                ImportAppendTree(callNode);
            }
            else
            {
                PushExpression(callNode);
            }
        }

        public void ImportRet(MethodDef method)
        {
            var hasReturnValue = method.HasReturnType;
            var retNode = new ReturnEntry();
            if (hasReturnValue)
            {
                var value = _stack.Pop();
                if (value.Kind != StackValueKind.Int32)
                {
                    throw new NotSupportedException("Return values of types other than short and int32 not supported yet");
                }
                retNode.Return = value;
            }
            ImportAppendTree(retNode);
        }
    }
}
