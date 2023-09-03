using ILCompiler.Compiler.EvaluationStack;
using System.Text;

namespace ILCompiler.Compiler
{
    public class LIRDumper : IStackEntryVisitor
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public string Dump(IList<BasicBlock> blocks)
        {
            _sb.Clear();
            foreach (var block in blocks)
            {
                _sb.AppendLine($"Block {block.Label}");
                _sb.AppendLine($"Block immediate dominator {block.ImmediateDominator?.Label}");

                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    currentNode.Accept(this);
                    currentNode = currentNode.Next;
                }
            }

            return _sb.ToString();
        }

        public void Visit(NativeIntConstantEntry entry)
        {
            _sb.AppendLine($"t{entry.TreeID,-3} = nativeintconst {entry.Value}");
        }

        public void Visit(Int32ConstantEntry entry)
        {
            _sb.AppendLine($"t{entry.TreeID,-3} = intconst {entry.Value}");
        }

        public void Visit(StringConstantEntry entry)
        {
            _sb.AppendLine($"t{entry.TreeID,-3} = intconst {entry.Value}");
        }

        public void Visit(StoreIndEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       storeind {entry.Addr.TreeID}");
        }

        public void Visit(JumpTrueEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Condition.TreeID}");
            _sb.AppendLine($"       jumptrue {entry.TargetLabel}");
        }

        public void Visit(JumpEntry entry)
        {
            _sb.AppendLine($"       jump {entry.TargetLabel}");
        }

        public void Visit(ReturnEntry entry)
        {
            if (entry.Return != null)
            {
                _sb.AppendLine($"       ┌──▌  t{entry.Return.TreeID}");
            }
            _sb.AppendLine($"       return");
        }

        public void Visit(BinaryOperator entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       ├──▌  t{entry.Op2.TreeID}");
            _sb.AppendLine($"t{entry.TreeID,-3} = {entry.Operation}");
        }

        public void Visit(CommaEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       ├──▌  t{entry.Op2.TreeID}");
            _sb.AppendLine($"t{entry.TreeID,-3} comma");
        }

        public void Visit(LocalVariableEntry entry)
        {
            _sb.AppendLine($"t{entry.TreeID,-3} = lclVar {entry.Type} V{entry.LocalNumber}");
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            _sb.AppendLine($"t{entry.TreeID,-3} = lclVarAddr {entry.Type} V{entry.LocalNumber}");
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       storelcl {entry.LocalNumber}");
        }

        public void Visit(PhiNode entry)
        {
            _sb.AppendLine($"       phi");
        }

        public void Visit(CallEntry entry)
        {
            var firstArg = true;
            foreach (var argument in entry.Arguments)
            {
                _sb.Append("       ");
                _sb.Append(firstArg ? "┌──▌" : "├──▌");
                _sb.Append("  ");
                _sb.AppendLine($"t{argument.TreeID}");
                firstArg = false;
            }

            if (entry.Type != VarType.Void)
            {
                _sb.AppendLine($"t{entry.TreeID,-3} = call {entry.TargetMethod}");
            }
            else
            {
                _sb.AppendLine($"       call {entry.TargetMethod}");
            }
        }

        public void Visit(IntrinsicEntry entry)
        {
            var firstArg = true;
            foreach (var argument in entry.Arguments)
            {
                _sb.Append("       ");
                _sb.Append(firstArg ? "┌──▌" : "├──▌");
                _sb.Append("  ");
                _sb.AppendLine($"t{argument.TreeID}");
                firstArg = false;
            }

            // TODO: Consider intrinsics that return data
            _sb.AppendLine($"       {entry.TargetMethod}");
        }

        public void Visit(CastEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       cast {entry.Type}");
        }

        public void Visit(UnaryOperator entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"t{entry.TreeID,-3} = {entry.Operation}");
        }

        public void Visit(IndirectEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"t{entry.TreeID, -3} = ind ${entry.Offset}");
        }

        public void Visit(FieldAddressEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       fieldAddr {entry.Name}");
        }

        public void Visit(StaticFieldEntry entry)
        {
            _sb.AppendLine($"       staticField {entry.Name}");
        }

        public void Visit(SwitchEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       switch {entry.JumpTable}");
        }

        public void Visit(AllocObjEntry entry)
        {
            _sb.AppendLine($"       allocObj {entry.Size}");
        }

        public void Visit(LocalHeapEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       lclheap");
        }

        public void Visit(IndexRefEntry entry)
        {
            throw new Exception("IndexRefEntry not valid in LIR");
        }

        public void Visit(PutArgTypeEntry entry)
        {
            _sb.AppendLine($"       ┌──▌  t{entry.Op1.TreeID}");
            _sb.AppendLine($"       putarg_type {entry.ArgType}");
        }
    }
}
