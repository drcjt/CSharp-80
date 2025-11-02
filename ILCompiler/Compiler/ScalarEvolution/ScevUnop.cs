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

        public override Scev Simplify(ScevContext context)
        {
            Scev op1 = Op1.Simplify(context);

            if (Type == op1.Type)
            {
                return op1;
            }

            if (op1 is ScevConstant cns1)
            {
                return context.NewConstant(Type, (short)cns1.Value);
            }

            if (op1 is ScevAddRec addRec)
            {
                Scev newStart = context.NewUnop(Operator, addRec.Start, Type).Simplify(context);
                Scev newStep = context.NewUnop(Operator, addRec.Step, Type).Simplify(context);
                return context.NewAddRec(newStart, newStep);
            }

            return this;
        }

        public override string ToString() => Operator switch
        {
            ScevOperator.Narrow => $"narrow<{Type.GetTypeSize()}>({Op1})",
            ScevOperator.ZeroExtend => $"zext{Type.GetTypeSize()}>({Op1})",
            ScevOperator.SignExtend => $"sext<{Type.GetTypeSize()}>({Op1})",
            _ => throw new ArgumentException("Unknown SCEV unary operator"),
        };
    }
}
