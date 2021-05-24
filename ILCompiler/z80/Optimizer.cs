using ILCompiler.z80.Interfaces;
using Microsoft.Extensions.Logging;

namespace ILCompiler.z80
{
    public class Optimizer : IOptimizer
    {
        private readonly ILogger<Optimizer> _logger;

        public Optimizer(ILogger<Optimizer> logger)
        {
            _logger = logger;
        }

        public void Optimize(IZ80Assembly assembly)
        {
            EliminatePushXXPopXX(assembly);
        }

        private void EliminatePushXXPopXX(IZ80Assembly assembly)
        {
            int unoptimizedInstructionCount = assembly.Count;
            if (assembly.Count > 1)
            {
                var lastInstruction = assembly[0];
                var currentInstruction = assembly[1];
                int count = 1;
                do
                {
                    if (lastInstruction.Opcode == Opcode.Push && currentInstruction.Opcode == Opcode.Pop
                        && lastInstruction.Operands == currentInstruction.Operands)
                    {
                        // Eliminate Push followed by Pop
                        assembly.RemoveAt(count - 1);
                        assembly.RemoveAt(count - 1);
                    }

                    lastInstruction = currentInstruction;
                    currentInstruction = assembly[++count];
                } while (count < assembly.Count - 1);
            }
            _logger.LogInformation($"Eliminated {unoptimizedInstructionCount - assembly.Count} instructions");

        }
    }
}
