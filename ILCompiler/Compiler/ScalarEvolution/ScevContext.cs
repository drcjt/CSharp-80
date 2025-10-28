using System.Diagnostics;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Ssa;

namespace ILCompiler.Compiler.ScalarEvolution
{
    internal class ScevContext
    {
        private FlowGraphNaturalLoop? _loop;
        private readonly LocalVariableTable _locals;
        private readonly Dictionary<StackEntry, Scev> _ephemeralCache = [];
        private bool _usingEphemeralCache = false;

        public ScevContext(LocalVariableTable locals)
        {
            _locals = locals;
        }

        public void ResetForLoop(FlowGraphNaturalLoop loop)
        {
            _loop = loop;
        }

        public Scev? Analyze(BasicBlock block, StackEntry tree) => Analyze(block, tree, 0);

        public static StackEntry? Materialize(Scev scev) => scev.Materialize();

        private const int MaxAnalysisDepth = 5;

        private Scev? Analyze(BasicBlock block, StackEntry tree, int depth)
        {
            if (!_ephemeralCache.TryGetValue(tree, out Scev? result))
            {
                if (depth <= MaxAnalysisDepth)
                {
                    result = AnalyzeNew(block, tree, depth);

                    if (_usingEphemeralCache)
                    {
                        _ephemeralCache[tree] = result!;
                    }
                }
            }

            return result;
        }

        private Scev? AnalyzeNew(BasicBlock block, StackEntry tree, int depth)
        {
            switch (tree)
            {
                case Int32ConstantEntry entry:
                    return CreateScevForConstant(entry);

                case LocalVariableCommon entry:
                {
                    LocalVariableDescriptor dsc = _locals[entry.LocalNumber];
                    LocalSsaVariableDescriptor ssaDsc = dsc.GetPerSsaData(entry.SsaNumber);

                    if (tree.Type != dsc.Type)
                    {
                        return null;
                    }

                    if (ssaDsc.Block is null || !_loop!.ContainsBlock(ssaDsc.Block))
                    {
                        return NewLocal(entry.LocalNumber, entry.SsaNumber);
                    }

                    if (ssaDsc.DefNode is null)
                    {
                        return null;
                    }

                    if (ssaDsc.DefNode.LocalNumber != entry.LocalNumber)
                    {
                        return null;
                    }

                    return Analyze(ssaDsc.Block, ssaDsc.DefNode.Data, depth + 1);
                }

                case PhiNode phiNode:
                {
                    if (block != _loop?.Header)
                    {
                        // Phi nodes outside the loop header are not considered
                        return null;
                    }

                    // Look for a primary induction variable
                    (PhiArg? enterSsa, PhiArg? backEdgeSsa) = FindEnterAndBackedgeSsa(phiNode);
                    if (enterSsa is null || backEdgeSsa is null)
                    {
                        return null;
                    }

                    ScevLocal enterScev = NewLocal(enterSsa.LocalNumber, enterSsa.SsaNumber);

                    LocalVariableDescriptor dsc = _locals[enterSsa.LocalNumber];
                    LocalSsaVariableDescriptor ssaDsc = dsc.GetPerSsaData(backEdgeSsa.SsaNumber);

                    if (ssaDsc.DefNode is null)
                    {
                        return null;
                    }

                    if (ssaDsc.DefNode.LocalNumber != enterSsa.LocalNumber)
                    {
                        return null;
                    }

                    Debug.Assert(ssaDsc.Block is not null);

                    // Try the simple but most common case first, for a direct add recurrence like i = i + 1
                    Scev? simpleAddRec = CreateSimpleAddRec(phiNode, enterScev, ssaDsc.DefNode.Data);
                    if (simpleAddRec is not null)
                    {
                        return simpleAddRec;
                    }

                    // Otherwise create a symbolic node representing the recurrence and analyze recursively.
                    ScevConstant symbolicAddRec = NewConstant(phiNode.Type, 0xdead);
                    _ephemeralCache[phiNode] = symbolicAddRec;

                    Scev? result;
                    if (_usingEphemeralCache)
                    {
                        result = Analyze(ssaDsc.Block, ssaDsc.DefNode.Data, depth + 1);
                    }
                    else
                    {
                        _usingEphemeralCache = true;
                        result = Analyze(ssaDsc.Block, ssaDsc.DefNode.Data, depth + 1);
                        _usingEphemeralCache = false;
                        _ephemeralCache.Clear();
                    }

                    if (result is null)
                    {
                        return null;
                    }

                    return MakeAddRecFromRecursiveScev(enterScev, result, symbolicAddRec);
                }

                case BinaryOperator binaryOperator:
                {
                    if (!OperationIsAddSubMultiplyOrLeftShift(binaryOperator.Operation))
                    {
                        return null;
                    }

                    Scev? op1 = Analyze(block, binaryOperator.Op1, depth + 1);
                    if (op1 is null)
                    {
                        return null;
                    }
                    Scev? op2 = Analyze(block, binaryOperator.Op2, depth + 1);
                    if (op2 is null)
                    {
                        return null;
                    }

                    if (binaryOperator.Operation == Operation.Sub)
                    {
                        // Change subtraction into addition of a negative constant
                        op2 = NewBinop(ScevOperator.Multiply, op2, NewConstant(op2.Type, -1));
                    }

                    ScevOperator oper = binaryOperator.Operation switch
                    {
                        Operation.Add => ScevOperator.Add,
                        Operation.Sub => ScevOperator.Add,
                        Operation.Mul => ScevOperator.Multiply,
                        Operation.Lsh => ScevOperator.LeftShift,
                        _ => throw new InvalidOperationException(),
                    };

                    return NewBinop(oper, op1, op2);
                }

                default:
                    return null;
            }
        }

        private static bool OperationIsAddSubMultiplyOrLeftShift(Operation operation) => operation switch
        {
            Operation.Add => true,
            Operation.Sub => true,
            Operation.Mul => true,
            Operation.Lsh => true,
            _ => false,
        };

        private ScevAddRec? MakeAddRecFromRecursiveScev(Scev startScev, Scev scev, Scev recursiveScev)
        {
            if (scev is not ScevBinop binop || binop.Operator != ScevOperator.Add)
            {
                return null;
            }

            Stack<Scev> addOperands = new();
            ((ScevBinop)scev).ExtractAddOperands(addOperands);

            int numberOfAppearances = FindNumberOfAppearances(recursiveScev, addOperands);
            if (numberOfAppearances == 0 || numberOfAppearances > 1)
            {
                return null;
            }

            Scev? step = null;
            for (int i = 0; i < addOperands.Count; i++)
            {
                Scev addOperand = addOperands.Bottom(i);
                if (addOperand == recursiveScev)
                {
                    continue;
                }

                if (step is null)
                {
                    step = addOperand;
                }
                else
                {
                    step = NewBinop(ScevOperator.Add, step, addOperand);
                }
            }

            return NewAddRec(startScev, step!);
        }

        private static int FindNumberOfAppearances(Scev recursiveScev, Stack<Scev> addOperands)
        {
            int numberOfApperances = 0;
            for (int i = 0; i < addOperands.Count; i++)
            {
                Scev addOperand = addOperands.Bottom(i);
                if (addOperand == recursiveScev)
                {
                    numberOfApperances++;
                }
                else
                {
                    ScevVisit result = addOperand.Visit(node => node == recursiveScev ? ScevVisit.Abort : ScevVisit.Continue);
                    if (result == ScevVisit.Abort)
                    {
                        numberOfApperances = 0;
                        break;
                    }
                }
            }

            return numberOfApperances;
        }

        private ScevAddRec? CreateSimpleAddRec(PhiNode phi, ScevLocal enterScev, StackEntry stepDef)
        {
            ScevAddRec? result = null;
            if (stepDef is BinaryOperator binOp && binOp.Operation == Operation.Add)
            {
                StackEntry op1 = binOp.Op1;
                StackEntry op2 = binOp.Op2;

                StackEntry? GetUseValue(StackEntry value)
                {
                    if (value is LocalVariableEntry localVariable)
                    {
                        LocalVariableCommon lcl = localVariable as LocalVariableCommon;
                        LocalVariableDescriptor lclDsc = _locals[lcl.LocalNumber];
                        LocalSsaVariableDescriptor ssaDsc = lclDsc.GetPerSsaData(lcl.SsaNumber);

                        if (ssaDsc.DefNode is not null)
                        {
                            return ssaDsc.DefNode.Data;
                        }
                    }

                    return null;
                }

                StackEntry? stepTree;
                if (GetUseValue(op1) == phi)
                {
                    stepTree = op2;
                }
                else if (GetUseValue(op2) == phi)
                {
                    stepTree = op1;
                }
                else
                {
                    // Not a simple IV, more complex than i = i + k
                    return null;
                }

                Scev? stepScev = CreateSimpleInvariantScev(stepTree);
                if (stepScev is not null)
                {
                    result = NewAddRec(enterScev, stepScev);
                }
            }

            return result;
        }

        private Scev? CreateSimpleInvariantScev(StackEntry tree)
        {
            if (tree.IsIntCnsOrI())
            {
                return CreateScevForConstant(tree);
            }

            if (tree is LocalVariableEntry localVariableEntry)
            {
                LocalVariableDescriptor dsc = _locals[localVariableEntry.LocalNumber];
                LocalSsaVariableDescriptor ssaDsc = dsc.GetPerSsaData(localVariableEntry.SsaNumber);

                if (ssaDsc.Block is null || !_loop!.ContainsBlock(ssaDsc.Block))
                {
                    return NewLocal(localVariableEntry.LocalNumber, localVariableEntry.SsaNumber);
                }
            }

            return null;
        }

        private static ScevConstant? CreateScevForConstant(StackEntry tree) => new ScevConstant(tree.Type, tree.GetIntConstant());

        public ScevAddRec NewAddRec(Scev start, Scev step) => new(start, step, _loop!);
        public ScevConstant NewConstant(VarType type, int value) => new(type, value);
        public ScevLocal NewLocal(int localNumber, int ssaNumber) => new(_locals[localNumber].Type, localNumber, ssaNumber, _locals);
        public ScevBinop NewBinop(ScevOperator op, Scev op1, Scev op2) => new(op, op1.Type, op1, op2);
        
        private (PhiArg? enterSsa, PhiArg? backEdgeSsa) FindEnterAndBackedgeSsa(PhiNode phiNode)
        {
            PhiArg? enterSsa = null;
            PhiArg? backEdgeSsa = null;
            foreach (PhiArg phiArg in phiNode.Arguments)
            {
                if (_loop!.ContainsBlock(phiArg.PredecessorBlock))
                {
                    if (backEdgeSsa is null || backEdgeSsa.SsaNumber == phiArg.SsaNumber)
                    {
                        backEdgeSsa = phiArg;
                    }
                }
                else
                {
                    if (enterSsa is null || enterSsa.SsaNumber == phiArg.SsaNumber)
                    {
                        enterSsa = phiArg;
                    }
                }
            }
            return (enterSsa, backEdgeSsa);
        }
    }
}
