using System.Text;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler
{
    public class TreeDumper : IStackEntryVisitor
    {
        private int _indent;
        private LocalVariableTable _locals = default!;
        private readonly StringBuilder _sb = new();

        public string Dump(Statement statement, LocalVariableTable locals)
        {
            _locals = locals;

            _sb.Clear();
            _indent = 0;
            statement.RootNode.Accept(this);
            return _sb.ToString();
        }

        public string Dump(IList<BasicBlock> blocks, LocalVariableTable locals)
        {
            _locals = locals;
            int statementId = 0;

            _sb.Clear();
            foreach (BasicBlock block in blocks)
            {
                string blockKind = GetBlockKind(block);
                string blockPreds = GetBlocks(block.Predecessors);
                string blockSuccs = GetBlocks(block.Successors);
                _sb.AppendLine($"{block.Label} [{block.StartOffset:0000}] ({blockKind}) preds={{{blockPreds}}} succs={{{blockSuccs}}}");

                foreach (Statement statement in block.Statements)
                {
                    _indent = 0;
                    _sb.AppendLine($"STMT{statementId:00000} [0x{statement.StartOffset:X3} ... 0x{statement.EndOffset:X3})");
                    _indent++;

                    statement.RootNode.Accept(this);
                    statementId++;
                }

                _sb.AppendLine();
            }
            return _sb.ToString();
        }

        private static string GetBlocks(IList<BasicBlock> blocks)
        {
            bool first = true;
            StringBuilder sb = new();
            foreach (BasicBlock block in blocks)
            {
                if (!first)
                {
                    sb.Append(',');
                }
                sb.Append(block.Label);
                first = false;
            }
            return sb.ToString();
        }

        private static string GetBlockKind(BasicBlock block) => 
            block.JumpKind switch
            {
                JumpKind.Return => "return",
                JumpKind.Always => "always",
                JumpKind.Conditional => "cond",
                JumpKind.Switch => "switch",
                _ => "unknown",
            };

        private static string GetLocalName(LocalVariableDescriptor local)
        {
            string result = "";
            if (string.IsNullOrEmpty(local.Name))
            {
                if (local.IsTemp)
                {
                    result = "(tmp)";
                }
            }
            else
            {
                result = $"({local.Name})";
            }

            return result;
        }

        private void PrintNodeText(StackEntry tree, string text)
        {
            string nodeText = new String(' ', _indent * 2) + text;
            _sb.AppendLine($"  [{tree.TreeID:000000}] ------------              *{nodeText}");
        }

        private void Print(StackEntry tree, string nodeName, VarType? type = null, string postText = "")
        {
            PrintNodeText(tree, $"{nodeName.ToUpper()} {(type ?? tree.Type).ToString().ToLower()} {postText}");
        }

        private void Print(StackEntry tree, string nodeName, VarType? type, int localNumber)
        {
            LocalVariableDescriptor local = _locals[localNumber];
            string localName = GetLocalName(local);
            Print(tree, nodeName, type, $"V{localNumber} {localName}");
        }

        public void Visit(NativeIntConstantEntry entry)
        {
            Print(entry, "CNS_INT", VarType.Ptr, entry.Value.ToString());
        }

        public void Visit(Int32ConstantEntry entry)
        {
            Print(entry, "CNS_INT", entry.Type, entry.Value.ToString());
        }

        public void Visit(StoreIndEntry entry)
        {
            Print(entry, $"STORE_IND offset={entry.FieldOffset} size={entry.ExactSize} vartype={entry.Type}");
            _indent++;
            entry.Addr.Accept(this);
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(JumpTrueEntry entry)
        {
            Print(entry, "JTRUE");
            _indent++;
            entry.Condition.Accept(this);
            _indent--;
        }

        public void Visit(JumpEntry entry)
        {
            Print(entry, "JMP");
        }

        public void Visit(TokenEntry entry)
        {
            Print(entry, $"TOKEN {entry.Field.Name}");
        }

        public void Visit(SwitchEntry entry)
        {
            Print(entry, $"SWITCH {entry.JumpTable}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(ReturnEntry entry)
        {
            Print(entry, "RETURN");
            if (entry.Return != null)
            {
                _indent++;
                entry.Return.Accept(this);
                _indent--;
            }
        }

        public void Visit(BinaryOperator entry)
        {
            Print(entry, entry.Operation.ToString());
            _indent++;
            entry.Op1.Accept(this);
            entry.Op2.Accept(this);
            _indent--;
        }

        public void Visit(CommaEntry entry)
        {
            Print(entry, "COMMA");
            _indent++;
            entry.Op1.Accept(this);
            entry.Op2.Accept(this);
            _indent--;
        }

        public void Visit(LocalVariableEntry entry)
        {
            Print(entry, "LCL_VAR", entry.Type, entry.LocalNumber);
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            Print(entry, $"LCL_VAR_ADDR", entry.Type, entry.LocalNumber);
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            Print(entry, "STORE_LCL_VAR", entry.Op1.Type, entry.LocalNumber);
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(CallEntry entry)
        {
            Print(entry, "CALL", entry.Type, entry.Method!.Name);
            _indent++;
            foreach (StackEntry argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            _indent--;
        }

        public void Visit(IntrinsicEntry entry)
        {
            Print(entry, "CALL", entry.Type, $"intrinsic {entry.TargetMethod}");
            _indent++;
            foreach (StackEntry argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            _indent--;
        }

        public void Visit(CastEntry entry)
        {
            Print(entry, "CAST");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(ArrayLengthEntry entry)
        {
            Print(entry, "ARRAYLENGTH");
            _indent++;
            entry.ArrayReference.Accept(this);
            _indent--;
        }

        public void Visit(NullCheckEntry entry)
        {
            Print(entry, "NULLCHECK");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(PutArgTypeEntry entry)
        {
            Print(entry, "PUTARG_TYPE", entry.ArgType);
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(UnaryOperator entry)
        {
            Print(entry, entry.Operation.ToString());
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(IndirectEntry entry)
        {
            Print(entry, $"IND {entry.Offset}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(FieldAddressEntry entry)
        {
            Print(entry, $"FLD_ADDR {entry.Name}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(SymbolConstantEntry entry)
        {
            Print(entry, $"SYMBOL {entry.Value}");
        }

        public void Visit(AllocObjEntry entry)
        {
            Print(entry, "ALLOCOBJ");
            _indent++;
            entry.EETypeNode.Accept(this);
            _indent--;
        }

        public void Visit(LocalHeapEntry entry)
        {
            Print(entry, "LCLHEAP");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(PhiNode entry)
        {
            Print(entry, "PHI ");
            _indent++;
            foreach (PhiArg argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            _indent--;
        }

        public void Visit(PhiArg entry)
        {
            Print(entry, "PHIARG");
        }

        public void Visit(IndexRefEntry entry)
        {
            Print(entry, "[]");
            _indent++;
            entry.ArrayOp.Accept(this);
            entry.IndexOp.Accept(this);
            _indent--;
        }

        public void Visit(BoundsCheck entry)
        {
            Print(entry, "BOUNDS_CHECK");
            _indent++;
            entry.Index.Accept(this);
            entry.ArrayLength.Accept(this);
            _indent--;
        }

        public void Visit(CatchArgumentEntry entry)
        {
            Print(entry, "CATCH_ARGUMENT");
        }

        public void Visit(ReturnExpressionEntry entry)
        {
            Print(entry, "RETURN_EXPRESSION");
            _indent++;
            entry.InlineCandidate.Accept(this);
            _indent--;
        }

        public void Visit(NothingEntry entry)
        {
            Print(entry, "NOTHING");
        }
    }
}
