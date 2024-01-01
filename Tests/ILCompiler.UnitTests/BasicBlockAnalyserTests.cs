using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ILCompiler.Tests
{
    [TestFixture]
    class BasicBlockAnalyserTests
    {
        private static MethodDesc BuildMethod(IList<Instruction> instructions)
        {
            instructions.UpdateInstructionOffsets();
            var methodDef = new MethodDefUser
            {
                Body = new CilBody(true, instructions, new List<ExceptionHandler>(), new List<Local>())
            };
            return new MethodDesc(methodDef);
        }

        [Test]
        public void FindBasicBlocks_WithNoBranches_CreatesBasicBlockWithAlwaysJumpKind()
        {
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Ldc_I4_0),
                new Instruction(OpCodes.Ldc_I4_S, 1234),
            });
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

            Assert.AreEqual(JumpKind.Always, basicBlocks[0].JumpKind);
        }

        [Test]
        public void FindBasicBlocks_WithOnlyReturn_CreatesBasicBlockWithReturnJumpKind()
        {
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Ret),
            });
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

            Assert.AreEqual(JumpKind.Return, basicBlocks[0].JumpKind);
        }

        [Test]
        public void FindBasicBlocks_WithSwitch_CreatesBasicBlockWithSwitchJumpKind()
        {
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Switch),
                new Instruction(OpCodes.Ret),
            });
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

            Assert.AreEqual(JumpKind.Switch, basicBlocks[0].JumpKind);
        }

        [Test]
        public void FindBasicBlocks_WithConditionalBranch_CreatesBasicBlockWithConditionalJumpKind()
        {
            var code = new List<Instruction>();
            var branchTarget = OpCodes.Nop.ToInstruction();
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(OpCodes.Brfalse.ToInstruction(branchTarget));
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(branchTarget);
            var method = BuildMethod(code);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

            Assert.AreEqual(JumpKind.Conditional, basicBlocks[0].JumpKind);
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
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

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
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

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
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

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
            var offsetToIndexMap = new Dictionary<int, int>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

            Assert.AreEqual(2, basicBlocks.Count(x => x != null));
            Assert.IsNotNull(basicBlocks[branchTarget.Offset]);
        }
    }
}
