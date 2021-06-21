using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public partial class ILImporter
    {
        public IList<BasicBlock> Import(Z80MethodCodeNode methodCodeNodeNeedingCode)
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

        public void ImportBranch(Code opcode, BasicBlock target, BasicBlock fallthrough)
        {
            // Gen tree generation and type checking
            if (opcode != Code.Br)
            {
                var op2 = _stack.Pop();
                if (op2.Kind != StackValueKind.Int16)
                {
                    throw new NotSupportedException("Boolean comparisonsonly supported using short as underlying type");
                }

                StackEntry op1;
                BinaryOp op;
                if (opcode != Code.Brfalse && opcode != Code.Brtrue)
                {
                    op1 = _stack.Pop();
                    if (op2.Kind != StackValueKind.Int16)
                    {
                        throw new NotSupportedException("Boolean comparisons only supported using short as underlying type");
                    }
                    op = BinaryOp.EQ + (opcode - Code.Beq);
                }
                else
                {
                    op1 = new Int16ConstantEntry((short)(opcode == Code.Brfalse ? 0 : 1));
                    op = BinaryOp.EQ;
                }
                op1 = new BinaryOperator(op, op1, op2, StackValueKind.Int16);
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
            // Gen tree generation and type checking
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

            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Binary operations on types other than short not supported yet");
            }

            BinaryOp binaryOp = opcode == Code.Add ? BinaryOp.ADD : (opcode == Code.Sub ? BinaryOp.SUB : BinaryOp.MUL);           
            var binaryExpr = new BinaryOperator(binaryOp, op1, op2, kind);
            PushExpression(binaryExpr);
        }

        public void ImportStoreIndirect(WellKnownType type)
        {
            // Gen tree generation and type checking
            var value = _stack.Pop();
            var addr = _stack.Pop();

            if (type != WellKnownType.SByte || addr.Kind != StackValueKind.Int16 || value.Kind != StackValueKind.Int16)
            {
                throw new NotSupportedException();
            }

            ImportAppendTree(new StoreIndEntry(addr, value));
        }

        public void ImportStoreVar(int index, bool argument)
        {
            // Gen tree generation and type checking
            var value = _stack.Pop();
            if (value.Kind != StackValueKind.Int16 && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException("Storing variables other than short or object refs not supported yet");
            }
            var node = new StoreLocalVariableEntry(index, value);
            ImportAppendTree(node);
        }

        public void ImportLoadVar(int index, bool argument)
        {
            // Gen tree generation and type checking
            var localNumber = _method.Parameters.Count + index;
            var node = new LocalVariableEntry(localNumber, StackValueKind.Int16);
            PushExpression(node);
        }

        public void ImportLdArg(int index)
        {
            // Gen tree generation and type checking

            var node = new LocalVariableEntry(index, StackValueKind.Int16);
            PushExpression(node);
        }

        public void ImportLoadInt(long value, StackValueKind kind)
        {
            // Gen tree generation and type checking
            if (kind == StackValueKind.Int16)
            {
                PushExpression(new Int16ConstantEntry(checked((short)value)));
            }
            else if (kind == StackValueKind.Int32)
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
            // Gen tree generation and type checking
            PushExpression(new StringConstantEntry(str));
        }

        private bool ImportIntrinsicCall(MethodDef methodToCall)
        {
            // Gen tree generation and type checking
            IList<StackEntry> arguments = new List<StackEntry>();
            for (int i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = _stack.Pop();
                arguments.Add(argument);
            }

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
                        if (argtype.FullName == "System.String")
                        {
                            targetMethodName = "WriteString";
                        }
                        else if (argtype.FullName == "System.Int16")
                        {
                            targetMethodName = "WriteInt16";
                        }
                        else
                        {
                            targetMethodName = "WriteChar";
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
            // Intrinsic calls
            if (methodToCall.IsIntrinsic())
            {
                if (!ImportIntrinsicCall(methodToCall))
                {
                    throw new NotSupportedException("Unknown intrinsic");
                }
                return;
            }

            // Gen tree generation and type checking
            StackEntry[] arguments = new StackEntry[methodToCall.Parameters.Count];
            for (int i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = _stack.Pop();
                arguments[methodToCall.Parameters.Count - i - 1] = argument;
            }

            // Not yet implemented methods with non void return type
            if (methodToCall.HasReturnType)
            {
                throw new NotSupportedException();
            }

            string targetMethod = "";
            if (methodToCall.IsPinvokeImpl)
            {
                targetMethod = methodToCall.ImplMap.Name;
            }
            else
            {
                targetMethod = _compilation.NameMangler.GetMangledMethodName(methodToCall);
            }
            var callNode = new CallEntry(targetMethod, arguments, StackValueKind.Unknown);
            ImportAppendTree(callNode);
        }

        public void ImportRet(MethodDef method)
        {
            // Gen tree generation and type checking
            var hasReturnValue = method.HasReturnType;
            var retNode = new ReturnEntry();
            if (hasReturnValue)
            {
                var value = _stack.Pop();
                if (value.Kind != StackValueKind.Int16)
                {
                    throw new NotSupportedException("Return values of types other than short not supported yet");
                }
                retNode.Return = value;
            }
            ImportAppendTree(retNode);
        }

        public void Append(Instruction instruction)
        {
            _currentBasicBlock.Instructions.Add(instruction);
        }
    }
}
