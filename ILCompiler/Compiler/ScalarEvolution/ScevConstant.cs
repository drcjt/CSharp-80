using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.ScalarEvolution
{
    internal record ScevConstant(VarType Type, int Value) : Scev(Type)
    {
        public override int? GetConstantValue() => Value;

        public override ScevVisit Visit(Func<Scev, ScevVisit> visitor) => visitor(this);

        public override StackEntry? Materialize() => Type.GenActualTypeIsI() ? new NativeIntConstantEntry((short)Value) : new Int32ConstantEntry(Value);

        public override string ToString() => Value.ToString();
    }
}
