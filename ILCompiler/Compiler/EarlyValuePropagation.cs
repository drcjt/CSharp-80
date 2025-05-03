using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.LinearIR;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler
{
    internal class EarlyValuePropagation : IEarlyValuePropagation
    {
        private readonly IFlowgraph _flowgraph;
        public EarlyValuePropagation(IFlowgraph flowgraph)
        {
            _flowgraph = flowgraph;
        }

        public void Run(IList<BasicBlock> blocks, LocalVariableTable locals)
        {
            foreach (var block in blocks)
            {
                foreach (var statement in block.Statements)
                {
                    bool isRewritten = false;
                    foreach (var tree in statement.TreeList)
                    {
                        var rewrittenTree = EarlyPropagationRewriteTree(block, tree, locals);
                        if (rewrittenTree != null)
                        {
                            isRewritten = true;
                        }
                    }

                    if (isRewritten)
                    {
                        // Update the evaluation order
                        _flowgraph.SetStatementSequence(statement);
                    }
                }
            }
        }

        private StackEntry? EarlyPropagationRewriteTree(BasicBlock block, StackEntry tree, LocalVariableTable locals)
        {
            if (tree is ArrayLengthEntry arrayLengthEntry)
            {
                if (arrayLengthEntry.ArrayReference is LocalVariableEntry localVariableEntry)
                {
                    var localNumber = localVariableEntry.LocalNumber;

                    var localVariableDescriptor = locals[localNumber];
                    if (localVariableDescriptor.InSsa)
                    {
                        var ssaNumber = localVariableEntry.SsaNumber;

                        var actualValue = PropagationGetValue(localNumber, ssaNumber, locals);

                        if (actualValue is not null)
                        {
                            var actualValueClone = actualValue.Duplicate();

                            // Replace tree with actualValueClone
                            tree.ReplaceWith(actualValueClone, block);

                            return tree;
                        }
                    }
                }
            }

            return null;
        }

        private StackEntry? PropagationGetValue(int localNumber, int ssaNumber, LocalVariableTable locals)
        {
            return PropagationGetValueRecursive(localNumber, ssaNumber, 0, locals);
        }

        private static StackEntry? PropagationGetValueRecursive(int localNumber, int ssaNumber, int walkDepth, LocalVariableTable locals)
        {
            // Bound the recursion
            if (walkDepth > 5)
            {
                return null;
            }

            var local = locals[localNumber];
            var ssaVariableDescriptor = local.GetPerSsaData(ssaNumber);

            var ssaDefStore = ssaVariableDescriptor.DefNode;

            StackEntry? value = null;

            if (ssaDefStore is not null)
            {
                if (ssaDefStore is StoreLocalVariableEntry storeLocalVariableEntry && 
                    storeLocalVariableEntry.LocalNumber == localNumber &&
                    storeLocalVariableEntry.Op1 is LocalVariableEntry defValue)
                {
                    var defValueLocalNumber = defValue.LocalNumber;
                    var defValueSsaNumber = defValue.SsaNumber;
                    // Recurse to find the value
                    value = PropagationGetValueRecursive(defValueLocalNumber, defValueSsaNumber, walkDepth + 1, locals);
                }
                else
                {
                    // This is the allocation of the array
                    var storeLocalVariable = (StoreLocalVariableEntry)ssaDefStore;
                    StackEntry? arrayLength = GetArrayLengthFromAllocation(storeLocalVariable.Op1);

                    if (arrayLength is not null && arrayLength.IsIntCnsOrI())
                    {
                        value = arrayLength;
                    }
                }
            }

            return value;
        }

        private static StackEntry? GetArrayLengthFromAllocation(StackEntry tree)
        {
            StackEntry? arrayLength = null;
            if (tree is CallEntry call && IsMethodNewArray(call.Method))
            {

                // Get the array length parameter from the call
                arrayLength = call.Arguments[1];
            }

            if (arrayLength is not null)
            {
                arrayLength = arrayLength is PutArgTypeEntry putArgTypeEntry
                    ? putArgTypeEntry.Op1
                    : arrayLength;
            }

            return arrayLength;
        }

        private static bool IsMethodNewArray(MethodDesc? method)
        {
            return method?.Name == "NewArray" && method.OwningType.FullName == "System.Runtime.RuntimeImports";
        }
    }
}
