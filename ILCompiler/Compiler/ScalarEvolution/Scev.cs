using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.ScalarEvolution
{
    internal enum ScevVisit
    {
        Abort,
        Continue,
    }

    internal abstract record Scev(VarType Type)
    {
        public virtual int? GetConstantValue() => null;
        public virtual Scev Simplify(ScevContext context) => this;
        public virtual StackEntry? Materialize() => null;

        public virtual ScevVisit Visit(Func<Scev, ScevVisit> visitor) => ScevVisit.Continue;

        public bool IsInvariant => Visit(scev => scev is ScevAddRec ? ScevVisit.Abort : ScevVisit.Continue) != ScevVisit.Abort;
    }
}
