namespace ILCompiler.Compiler.ScalarEvolution
{
    internal record ScevAddRec(Scev Start, Scev Step, FlowGraphNaturalLoop Loop) : Scev(Start.Type)
    {
        public override ScevVisit Visit(Func<Scev, ScevVisit> visitor)
        {
            if (visitor(this) == ScevVisit.Abort)
            {
                return ScevVisit.Abort;
            }

            if (Start.Visit(visitor) == ScevVisit.Abort)
            {
                return ScevVisit.Abort;
            }

            return Step.Visit(visitor);
        }

        public override Scev Simplify(ScevContext context)
        {
            Scev start = Start.Simplify(context);
            Scev step = Step.Simplify(context);
            return (start ==Start) && (step == Step) ? this : context.NewAddRec(start, step);
        }

        public override string ToString() => $"<L{Loop.Index}, {Start}, {Step}>";
    }
}
