namespace ILCompiler.Compiler.ScalarEvolution
{
    internal record ScevUnop(ScevOperator Operator, VarType Type, Scev Op1) : Scev(Type)
    {
        public override ScevVisit Visit(Func<Scev, ScevVisit> visitor)
        {
            if (visitor(this) == ScevVisit.Abort)
            {
                return ScevVisit.Abort;
            }

            return Op1.Visit(visitor);
        }

        public override string ToString() => $"{(Operator == ScevOperator.ZeroExtend ? 'z' : 's')}ext<{Type.GetTypeSize()}>({Op1})" ;
    }
}
