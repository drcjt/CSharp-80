using ILCompiler.Compiler.EvaluationStack;
using System.Text;

namespace ILCompiler.Compiler
{
    public class TreeDumper : IStackEntryVisitor
    {
        private static int _indent;

        private StringBuilder _sb = new StringBuilder();

        public string Dump(IList<BasicBlock> blocks)
        {
            _sb.Clear();
            foreach (var block in blocks)
            {
                _sb.AppendLine($"Block {block.Label}");

                _sb.Append($"Block successors: ");
                foreach (var successor in block.Successors)
                {
                    _sb.Append($"${successor.Label}, ");
                }
                _sb.AppendLine();

                foreach (var statement in block.Statements)
                {
                    _indent = 0;
                    _sb.AppendLine("▌ stmtExpr");
                    _indent++;

                    statement.Accept(this);
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
            Print($"intconst {entry.Value}");
        }

        public void Visit(StringConstantEntry entry)
        {
            Print($"strconst {entry.Value}");
        }

        public void Visit(StoreIndEntry entry)
        {
            _indent++;
            entry.Addr.Accept(this);
            _indent--;
            Print($"storeind offset={entry.FieldOffset} size={entry.ExactSize}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(JumpTrueEntry entry)
        {
            Print($"jumptrue {entry.TargetLabel}");
            _indent++;
            entry.Condition.Accept(this);
            _indent--;
        }

        public void Visit(JumpEntry entry)
        {
            Print($"jump {entry.TargetLabel}");
        }

        public void Visit(SwitchEntry entry)
        {
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
            Print($"switch {entry.JumpTable}");
        }

        public void Visit(ReturnEntry entry)
        {
            Print("return");
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

        public void Visit(LocalVariableEntry entry)
        {
            Print($"lclVar {entry.Kind} V{entry.LocalNumber}");
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            Print($"lclVarAddr V{entry.LocalNumber}");
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            Print($"storelcl {entry.LocalNumber}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(CallEntry entry)
        {
            Print($"call {entry.TargetMethod}");
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
            Print($"cast {entry.DesiredType}");
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
            Print($"ind {entry.Offset}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(FieldAddressEntry entry)
        {
            Print($"fieldAddr {entry.Name}");
            _indent++;
            entry.Op1.Accept(this);
            _indent--;
        }

        public void Visit(AllocObjEntry entry)
        {
            Print($"allocObj {entry.Size}");
        }
    }
}
