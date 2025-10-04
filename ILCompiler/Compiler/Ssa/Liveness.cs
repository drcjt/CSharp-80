using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Ssa;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    internal static class Liveness
    {
        public static void LocalVarLiveness(IList<BasicBlock> blocks, LocalVariableTable locals, ILogger<SsaBuilder> logger)
        {
            LocalVarLivenessInit(locals);

            // Figure out use/def info for all basic blocks
            PerBlockLocalVarLiveness(blocks, logger);
            InterBlockLocalVarLiveness(blocks, locals);
        }

        private static void LocalVarLivenessInit(LocalVariableTable locals)
        {
            SetTrackedVariables(locals);

            // Mark all local variables as not requiring explicit initialization
            // Liveness analysis will determine local variables that do need to be initialized
            foreach (var localVariable in locals)
            {
                localVariable.MustInit = false;
            }
        }

        /// <summary>
        /// Determine which local variables will be tracked
        /// </summary>
        /// <param name="localVariableTable"></param>
        private static void SetTrackedVariables(LocalVariableTable locals)
        {
            foreach (var localVariable in locals)
            {
                localVariable.Tracked = true;

                // Don't track local variables which are address exposed
                if (localVariable.AddressExposed)
                {
                    localVariable.Tracked = false;
                }
            }
        }

        private static void InterBlockLocalVarLiveness(IList<BasicBlock> blocks, LocalVariableTable locals)
        {
            // Compute the IN and OUT sets using classic liveness algorithm
            LiveVarAnalyzer.AnalyzeLiveVars(blocks);

            // Set which local variable must be initialized
            for (var lclNum = 0; lclNum < locals.Count; lclNum++)
            {
                if (!locals[lclNum].IsParameter && blocks[0].LiveIn.IsMember(lclNum))
                {
                    locals[lclNum].MustInit = true;
                }
            }
        }

        private static void PerBlockLocalVarLiveness(IList<BasicBlock> blocks, ILogger<SsaBuilder> logger)
        {
            foreach (var block in blocks)
            {
                var useSet = VariableSet.Empty;
                var defSet = VariableSet.Empty;

                // Enumerate nodes in each statement in evaluation order

                foreach (var statement in block.Statements)
                {
                    // Filter out phi definitions
                    if (statement.IsPhiDefinition)
                        continue;

                    foreach (var node in statement.TreeList)
                    {
                        PerNodeLocalVarLiveness(node, useSet, defSet);
                    }
                }

                block.VarDef = defSet;
                block.VarUse = useSet;

                // Dump use def details
                var allVars = VariableSet.Union(defSet, useSet);

                logger.LogDebug(" USE({count}={curUseVarSet}", useSet.Count, useSet.DisplayVarSet(allVars));
                logger.LogDebug(" DEF({count}={defUseVarSet}", defSet.Count, defSet.DisplayVarSet(allVars));
            }
        }

        /// <summary>
        /// Calls MarkUseDef for any local variables encountered
        /// </summary>
        /// <param name="node"></param>
        private static void PerNodeLocalVarLiveness(StackEntry node, VariableSet useSet, VariableSet defSet)
        {
            // For LocalVariableEntry, LocalVariableAddressEntry, StoreLocalVariableEntry, StoreIndEntry??, FieldAddressEntry?
            if (node is LocalVariableCommon localVarNode)
            {
                MarkUseDef(localVarNode, useSet, defSet);
            }
        }

        private static void MarkUseDef(LocalVariableCommon tree, VariableSet useSet, VariableSet defSet)
        {
            // Assignment is a definition, everything else is a use.

            // Should we also check for StoreIndEntry which is generated from Stfld import??
            // Are these really partial definitions e.g. struct field is assigned to s.f = ...

            var isDef = tree is StoreLocalVariableEntry;
            var isUse = !isDef;

            var localNumber = tree.LocalNumber;
            if (isUse && !defSet.IsMember(localNumber))
            {
                useSet.AddElem(localNumber);
            }

            if (isDef)
            {
                defSet.AddElem(localNumber);
            }
        }
    }
}
