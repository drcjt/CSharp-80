namespace ILCompiler.Compiler.EvaluationStack
{
    // ASG
    public class AssignmentEntry : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public AssignmentEntry(StackEntry op1, StackEntry op2) : base(op1.Kind)
        {
            Op1 = op1;
            Op2 = op2;
        }
    }
}
