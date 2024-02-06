using ILCompiler.Compiler.EvaluationStack;
using System.Text;

namespace ILCompiler.Compiler
{
    public class TreeDumper : IStackEntryVisitor
    {
        private int _indent;

        private readonly StringBuilder _sb = new StringBuilder();

        public string Dump(IList<BasicBlock> blocks)
        {
            var stmtId = 0;

            _sb.Clear();
            foreach (var block in blocks)
            {
                _sb.AppendLine($"BLOCK {block.Label}");

                _sb.Append($"BLOCK SUCCESSORS: ");
                foreach (var successor in block.Successors)
                {
                    _sb.Append($"${successor.Label}, ");
                }
                _sb.AppendLine();

                foreach (var statement in block.Statements)
                {
                    _indent = 0;
                    _sb.AppendLine($"STMT{stmtId}");
                    _indent++;

                    statement.Accept(this);
                    stmtId++;
                }
            }
            return _sb.ToString();
        }

        private void Print(string text)
        {
            _sb.AppendLine(new String(' ', _indent * 3) + "▌ " + text);
        }

        public void Visit(NativeIntConstantEntry entry)
        {
            Print($"nativeintconst {entry.Value}");
        }

        public void Visit(Int32ConstantEntry entry)
        {
            Print($"CNS_INT {entry.Value}");
        }

        public void Visit(StoreIndEntry entry)
        {
            _indent++;
            entry.Addr.Accept(this);
            _indent--;
            Print($"STORE_IND offset={entry.FieldOffset} size={entry.ExactSize} vartype={entry.Type}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(JumpTrueEntry entry)
        {
            Print($"JCMP {entry.TargetLabel}");
            _indent++;
            entry.Condition.Accept(this);
            _indent--;
        }

        public void Visit(JumpEntry entry)
        {
            Print($"JMP {entry.TargetLabel}");
        }

        public void Visit(SwitchEntry entry)
        {
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
            Print($"SWITCH {entry.JumpTable}");
        }

        public void Visit(ReturnEntry entry)
        {
            Print("RETURN");
            if (entry.Return != null)
            {
                _indent++;
                entry.Return.Accept(this);
                _indent--;
            }
        }

        public void Visit(BinaryOperator entry)
        {
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
            Print($"{entry.Operation}");
            _indent++;
            entry.Op2.Accept(this);
            _indent--;
        }

        public void Visit(CommaEntry entry)
        {
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
            Print("COMMA");
            _indent++;
            entry.Op2.Accept(this);
            _indent--;
        }

        public void Visit(LocalVariableEntry entry)
        {
            Print($"LCL_VAR {entry.Type} V{entry.LocalNumber}");
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            Print($"LCL_VAR_ADDR V{entry.LocalNumber}");
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            Print($"STORE_LCL_VAR {entry.LocalNumber}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(CallEntry entry)
        {
            Print($"CALL {entry.TargetMethod}");
            _indent++;
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            _indent--;
        }

        public void Visit(IntrinsicEntry entry)
        {
            Print($"{entry.TargetMethod}");
            _indent++;
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            _indent--;
        }

        public void Visit(CastEntry entry)
        {
            Print($"CAST {entry.Type}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(NullCheckEntry entry)
        {
            Print($"NULLCHECK");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(PutArgTypeEntry entry)
        {
            Print($"PUTARG_TYPE {entry.ArgType}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(UnaryOperator entry)
        {
            Print($"{entry.Operation}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(IndirectEntry entry)
        {
            Print($"IND {entry.Offset}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(FieldAddressEntry entry)
        {
            Print($"FLD_ADDR {entry.Name}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(SymbolConstantEntry entry)
        {
            Print($"SYMBOL {entry.Value}");
        }

        public void Visit(AllocObjEntry entry)
        {
            Print($"ALLOCOBJ {entry.Size}");
        }

        public void Visit(LocalHeapEntry entry)
        {
            Print($"LCLHEAP");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(PhiNode entry)
        {
            Print($"PHI ");
            _indent++;
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            _indent--;
        }

        public void Visit(PhiArg entry)
        {
            Print($"PHIARG V{entry.LocalNumber:00} ssa{entry.SsaNumber} {entry.Type}");
        }

        public void Visit(IndexRefEntry entry)
        {
            _indent++;
            entry.IndexOp.Accept(this);
            _indent--;
            Print($"[]");
            _indent++;
            entry.ArrayOp.Accept(this);
            _indent--;
        }

        public void Visit(BoundsCheck entry)
        {
            Print($"BOUNDS_CHECK");
            _indent++;
            entry.Index.Accept(this);
            entry.ArrayLength.Accept(this);
            _indent--;
        }

        public void Visit(CatchArgumentEntry entry)
        {
            Print($"CATCH_ARGUMENT");
        }
    }
}
