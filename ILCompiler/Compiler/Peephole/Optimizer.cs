using ILCompiler.Compiler.Emit;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.Peephole
{
    public class Optimizer
    {
        private readonly ILogger<Optimizer> _logger;

        public Optimizer(ILogger<Optimizer> logger)
        {
            _logger = logger;
        }

        public void Optimize(IList<Instruction> instructions)
        {
            int removedInstructions;
            do
            {
                removedInstructions = EliminatePushXXPopXX(instructions);
            } while (removedInstructions > 0);
            removedInstructions = RemoveJumpsToNextInstruction(instructions);

            _logger.LogDebug("Eliminated {eliminatedInstructions} instructions", removedInstructions);
        }

        private static int RemoveJumpsToNextInstruction(IList<Instruction> instructions)
        {
            var removedInstructions = 0;
            Instruction currentInstruction;
            int count = 0;

            do
            {
                currentInstruction = instructions[count];
                if (currentInstruction != null && currentInstruction.Opcode == Opcode.Jp
                    && count < instructions.Count && currentInstruction.Label == null)
                {
                    var target = currentInstruction.Op0?.Label;
                    var nextInstruction = instructions[count + 1];
                    if (target == nextInstruction.Label)
                    {
                        instructions[count] = Instruction.CreateComment($"\tJP {target?.ToUpper()}");
                        removedInstructions++;
                    }
                }

                count++;
            } while (count < instructions.Count - 1);

            return removedInstructions;
        }

        private static int EliminatePushXXPopXX(IList<Instruction> instructions)
        {
            int removedInstructions = 0;
            Instruction? lastInstruction = null;
            int lastInstructionIndex = 0;
            Instruction currentInstruction;
            var currentInstructionIndex = 0;

            do
            {
                currentInstruction = instructions[currentInstructionIndex];

                if (lastInstruction?.Opcode == Opcode.Push && currentInstruction.Opcode == Opcode.Pop
                    && lastInstruction?.Op0?.Register == currentInstruction.Op0?.Register &&
                    currentInstruction.Label == null && lastInstruction?.Label == null)
                {
                    // Eliminate Push followed by Pop
                    instructions[lastInstructionIndex] = Instruction.CreateComment($"\tPUSH {lastInstruction?.Op0?.Register}");
                    instructions[currentInstructionIndex] = Instruction.CreateComment($"\tPOP {lastInstruction?.Op0?.Register}");
                    removedInstructions += 2;

                    // Last Instruction is now one before the Push just eliminated
                    lastInstructionIndex = lastInstructionIndex > 0 ? lastInstructionIndex-- : 0;
                    lastInstruction = lastInstructionIndex > 0 ? instructions[lastInstructionIndex] : null;
                }
                else
                {
                    lastInstruction = currentInstruction;
                    lastInstructionIndex = currentInstructionIndex;
                }

                currentInstructionIndex++;

                // Skip instructions with no opcode
                while (currentInstructionIndex < instructions.Count && instructions[currentInstructionIndex].Opcode == Opcode.None)
                {
                    currentInstructionIndex++;
                }
            } while (currentInstructionIndex < instructions.Count);

            return removedInstructions;
        }
    }
}
