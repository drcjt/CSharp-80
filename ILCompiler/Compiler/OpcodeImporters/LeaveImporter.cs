using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LeaveImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.leave && instruction.Opcode != ILOpcode.leave_s) return false;

            Instruction target = (Instruction)instruction.Operand;
            BasicBlock targetBlock = importer.BasicBlocks[(int)target.Offset];

            // Find all EH clauses that require finally funclets to run when leaving the current instruction
            List<EHClause> candidates = importer.EHClauses
                .Where(eh => FinallyMustRun(eh, instruction.Offset, target.Offset))
                .ToList();

            // Sort candidates by the size of their try block, so that we run innermost finally funclets first
            candidates.Sort((a, b) => a.TrySize.CompareTo(b.TrySize));

            // Run finally funclets in order from innermost to outermost
            foreach (EHClause candidate in candidates)
            {
                // Call the finally funclet
                string finallyFunclet = candidate.HandlerBegin.Label;
                CallEntry call = new(finallyFunclet, [], VarType.Void, 0);
                importer.ImportAppendTree(call);
            }

            importer.ImportFallThrough(targetBlock);

            importer.StopImporting = true;

            return true;
        }

        private static bool FinallyMustRun(EHClause eh, uint leaveIL, uint targetIL) =>
            eh.Kind == EHClauseKind.Fault &&
            eh.InTry(leaveIL) &&
            !eh.InTry(targetIL);
    }
}
