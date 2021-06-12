using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ILCompiler.Tests
{
    [TestFixture]
    class BasicBlockAnalyserTests
    {
        private static MethodDef BuildMethod(IList<Instruction> instructions)
        {
            instructions.UpdateInstructionOffsets();
            return new MethodDefUser
            {
                Body = new CilBody(true, instructions, new List<ExceptionHandler>(), new List<Local>())
            };
        }

        [Test]
        public void FindBasicBlocks_WithNoBranches_IdentifiesSingleBasicBlock()
        {
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Ldc_I4_0),
                new Instruction(OpCodes.Ldc_I4_S, 1234),
            });
            var basicBlockAnalyser = new BasicBlockAnalyser(method);

            basicBlockAnalyser.FindBasicBlocks();

            var basicBlocks = basicBlockAnalyser.BasicBlocks;
            Assert.AreEqual(1, basicBlocks.Count(x => x != null));
            Assert.IsNotNull(basicBlocks[0]);
        }

        [Test]
        public void FindBasicBlocks_WithConditionalBranches_IdentifiesBlockAtTargetOfBranch()
        {
            var code = new List<Instruction>();
            var branchTarget = OpCodes.Nop.ToInstruction();
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(OpCodes.Brfalse.ToInstruction(branchTarget));
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(branchTarget);
            var method = BuildMethod(code);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);

            basicBlockAnalyser.FindBasicBlocks();

            var basicBlocks = basicBlockAnalyser.BasicBlocks;
            Assert.AreEqual(3, basicBlocks.Count(x => x != null));
            Assert.IsNotNull(basicBlocks[branchTarget.Offset]);
        }

        [Test]
        public void FindBasicBlocks_WithConditionalBranches_IdentifiesBlockFollowingBranch()
        {
            var code = new List<Instruction>();
            var branchTarget = OpCodes.Nop.ToInstruction();
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(OpCodes.Brfalse.ToInstruction(branchTarget));
            var instructionAfterBranch = OpCodes.Nop.ToInstruction();
            code.Add(instructionAfterBranch);
            code.Add(branchTarget);
            var method = BuildMethod(code);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);

            basicBlockAnalyser.FindBasicBlocks();

            var basicBlocks = basicBlockAnalyser.BasicBlocks;
            Assert.AreEqual(3, basicBlocks.Count(x => x != null));
            Assert.IsNotNull(basicBlocks[instructionAfterBranch.Offset]);
        }

        [Test]
        public void FindBasicBlocks_WithUnconditionalBranch_IdentifiesBlockAtTargetOfBranch()
        {
            var code = new List<Instruction>();
            var branchTarget = OpCodes.Nop.ToInstruction();
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(branchTarget);
            code.Add(OpCodes.Br.ToInstruction(branchTarget));
            var method = BuildMethod(code);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);

            basicBlockAnalyser.FindBasicBlocks();

            var basicBlocks = basicBlockAnalyser.BasicBlocks;
            Assert.AreEqual(2, basicBlocks.Count(x => x != null));
            Assert.IsNotNull(basicBlocks[branchTarget.Offset]);
        }
    }
}
