namespace ILCompiler.Compiler.ScalarEvolution
{
    internal record ScevConstant(VarType Type, int Value) : Scev(Type)
    {
        public override int? GetConstantValue() => Value;

        public override ScevVisit Visit(Func<Scev, ScevVisit> visitor) => visitor(this);

        public override string ToString() => Value.ToString();
    }
}
