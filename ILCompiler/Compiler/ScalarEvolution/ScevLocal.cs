using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Ssa;

namespace ILCompiler.Compiler.ScalarEvolution
{
    internal record ScevLocal(VarType Type, int LocalNumber, int SsaNumber, LocalVariableTable Locals) : Scev(Type)
    {
        public override int? GetConstantValue()
        {
            LocalVariableDescriptor dsc = Locals[LocalNumber];
            LocalSsaVariableDescriptor ssaDsc = dsc.GetPerSsaData(SsaNumber);
            LocalVariableCommon? defNode = ssaDsc.DefNode;
            if (defNode is not null && defNode.Data.IsIntCnsOrI())
            {
                return defNode.Data.GetIntConstant();
            }
            return null;
        }

        public override Scev Simplify(ScevContext context)
        {
            int? cns = GetConstantValue();
            if (cns is not null)
            {
                return context.NewConstant(Type, cns.Value);
            }
            return this;
        }

        public override ScevVisit Visit(Func<Scev, ScevVisit> visitor) => visitor(this);

        public override string ToString() => $"V{LocalNumber}.{SsaNumber}";
    }
}
