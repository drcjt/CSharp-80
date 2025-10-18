namespace ILCompiler.Compiler.ScalarEvolution
{
    internal record ScevBinop(ScevOperator Operator, VarType Type, Scev Op1, Scev Op2) : ScevUnop(Operator, Type, Op1)
    {
        public override ScevVisit Visit(Func<Scev, ScevVisit> visitor)
        {
            if (visitor(this) == ScevVisit.Abort)
            {
                return ScevVisit.Abort;
            }

            if (Op1.Visit(visitor) == ScevVisit.Abort)
            {
                return ScevVisit.Abort;
            }

            return Op2.Visit(visitor);
        }

        public override Scev Simplify(ScevContext context)
        {
            Scev op1 = Op1.Simplify(context);
            Scev op2 = Op2.Simplify(context);

            // Propagate AddRec to the left side and constants to the right side
            if (Operator == ScevOperator.Add || Operator == ScevOperator.Multiply)
            {
                if (op2 is ScevAddRec && op1 is not ScevAddRec)
                {
                    (op1, op2) = (op2, op1);
                }

                if (op1 is ScevConstant && op2 is not ScevConstant)
                {
                    (op1, op2) = (op2, op1);
                }
            }

            if (op1 is ScevAddRec addRec)
            {
                Scev newStart = context.NewBinop(Operator, addRec.Start, op2).Simplify(context);
                Scev newStep = (Operator == ScevOperator.Multiply || Operator == ScevOperator.LeftShift)
                    ? context.NewBinop(Operator, addRec.Step, op2).Simplify(context)
                    : addRec.Step;
                return context.NewAddRec(newStart, newStep);
            }
            if (op1 is ScevConstant && op2 is ScevConstant)
            {
                ScevConstant cns1 = (ScevConstant)op1;
                ScevConstant cns2 = (ScevConstant)op2;
                int newValue = FoldBinop(Operator, cns1.Value, cns2.Value);

                return context.NewConstant(Type, newValue);
            }
            else if (op2 is ScevConstant cns2)
            {
                // a +/<< 0 = a
                if ((Operator == ScevOperator.Add || Operator == ScevOperator.LeftShift) && cns2.Value == 0)
                {
                    return op1;
                }

                // (a + c1) + c2 = a + (c1 + c2)
                if (Operator == ScevOperator.Add)
                {
                    if (op1 is ScevBinop binopOp1 && binopOp1.Operator == ScevOperator.Add && binopOp1.Op2 is ScevConstant)
                    {
                        ScevBinop newOp2 = context.NewBinop(ScevOperator.Add, binopOp1.Op2, cns2);
                        ScevBinop newAdd = context.NewBinop(ScevOperator.Add, binopOp1.Op1, newOp2);
                        return newAdd.Simplify(context);
                    }
                }

                if (Operator == ScevOperator.Multiply)
                {
                    // a * 0 = 0
                    if (cns2.Value == 0)
                    {
                        return cns2;
                    }

                    // a * 1 = a
                    if (cns2.Value == 1)
                    {
                        return op1;
                    }

                    // (a * c1) * c2 = a * (c1 * c2)
                    if (op1 is ScevBinop binopOp1 && binopOp1.Operator == ScevOperator.Multiply && binopOp1.Op2 is ScevConstant)
                    {
                        ScevBinop newOp2 = context.NewBinop(ScevOperator.Multiply, binopOp1.Op2, cns2);
                        ScevBinop newMul = context.NewBinop(ScevOperator.Multiply, binopOp1.Op1, newOp2);
                        return newMul.Simplify(context);
                    }
                }
            }
            else if (op1 is ScevConstant cns1)
            {
                // 0 << a = 0
                if (Operator == ScevOperator.LeftShift && cns1.Value == 0)
                {
                    return cns1;
                }
            }

            if (Operator == ScevOperator.Add)
            {
                // (a + c1) + (b + c2) = (a + b) + (c1 + c2)
                if ((op1 is ScevBinop binopOp1 && binopOp1.Operator == ScevOperator.Add && binopOp1.Op2 is ScevConstant) &&
                    (op2 is ScevBinop binopOp2 && binopOp2.Operator == ScevOperator.Add && binopOp2.Op2 is ScevConstant))
                {
                    ScevBinop newOp1 = context.NewBinop(ScevOperator.Add, binopOp1.Op1, binopOp2.Op1);
                    ScevBinop newOp2 = context.NewBinop(ScevOperator.Add, binopOp1.Op2, binopOp2.Op2);
                    ScevBinop newAdd = context.NewBinop(ScevOperator.Add, newOp1, newOp2);
                    return newAdd.Simplify(context);
                }
            }

            return (op1 == Op1) && (op2 == Op2) ? this : context.NewBinop(Operator, op1, op2);
        }

        private static int FoldBinop(ScevOperator oper, int op1, int op2) => oper switch
        {
            ScevOperator.Add => op1 + op2,
            ScevOperator.Multiply => op1 * op2,
            ScevOperator.LeftShift => op1 << op2,
            _ => throw new InvalidOperationException(),
        };

        public void ExtractAddOperands(Stack<Scev> operands)
        {
            if (Op1 is ScevBinop binopOp1 && binopOp1.Operator == ScevOperator.Add)
            {
                binopOp1.ExtractAddOperands(operands);
            }
            else
            {
                operands.Push(Op1);
            }

            if (Op2 is ScevBinop binopOp2 && binopOp2.Operator == ScevOperator.Add)
            {
                binopOp2.ExtractAddOperands(operands);
            }
            else
            {
                operands.Push(Op2);
            }
        }

        public override string ToString() => $"({Op1}{OpToString()}{Op2})";

        private string OpToString() => Operator switch
        {
            ScevOperator.Add => "+",
            ScevOperator.Multiply => "*",
            ScevOperator.LeftShift => "<<",
            _ => throw new InvalidOperationException(),
        };
    }
}
