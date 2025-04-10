using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Compiler;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ILCompiler.IL;

namespace ILCompiler.Tests
{
    [TestFixture]
    class BasicBlockAnalyserTests
    {
        private static MethodDesc BuildMethod(IList<Instruction> instructions, DnlibModule module)
        {
            instructions.UpdateInstructionOffsets();
            var methodDef = new MethodDefUser
            {
                Body = new CilBody(true, instructions, new List<ExceptionHandler>(), new List<Local>())
            };

            return module.Create(methodDef);
        }

        private DnlibModule CreateModule()
        {
            return new DnlibModule(
                new TypeSystemContext(new Configuration()), 
                new Compiler.CorLibModuleProvider(), 
                new RTILProvider());
        }

        [Test]
        public void FindBasicBlocks_WithNoBranches_CreatesBasicBlockWithAlwaysJumpKind()
        {
            var module = CreateModule();
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Ldc_I4_0),
                new Instruction(OpCodes.Ldc_I4_S, 1234),
            }, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks[0].JumpKind, Is.EqualTo(JumpKind.Always));
        }

        [Test]
        public void FindBasicBlocks_WithOnlyReturn_CreatesBasicBlockWithReturnJumpKind()
        {
            var module = CreateModule();
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Ret),
            }, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks[0].JumpKind, Is.EqualTo(JumpKind.Return));
        }

        [Test]
        public void FindBasicBlocks_WithSwitch_CreatesBasicBlockWithSwitchJumpKind()
        {
            var module = CreateModule();
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Switch, System.Array.Empty<Instruction>()),
                new Instruction(OpCodes.Ret),
            }, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks[0].JumpKind, Is.EqualTo(JumpKind.Switch));
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
            var module = CreateModule();
            var method = BuildMethod(code, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks[0].JumpKind, Is.EqualTo(JumpKind.Conditional));
        }

        [Test]
        public void FindBasicBlocks_WithNoBranches_IdentifiesSingleBasicBlock()
        {
            var module = CreateModule();
            var method = BuildMethod(new List<Instruction>()
            {
                new Instruction(OpCodes.Ldc_I4_0),
                new Instruction(OpCodes.Ldc_I4_S, 1234),
            }, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks.Count(x => x != null), Is.EqualTo(1));
            Assert.That(basicBlocks[0], Is.Not.Null);
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
            var module = CreateModule();
            var method = BuildMethod(code, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks.Count(x => x != null), Is.EqualTo(3));
            Assert.That(basicBlocks[branchTarget.Offset], Is.Not.Null);
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
            var module = CreateModule();
            var method = BuildMethod(code, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks.Count(x => x != null), Is.EqualTo(3));
            Assert.That(basicBlocks[instructionAfterBranch.Offset], Is.Not.Null);
        }

        [Test]
        public void FindBasicBlocks_WithUnconditionalBranch_IdentifiesBlockAtTargetOfBranch()
        {
            var code = new List<Instruction>();
            var branchTarget = OpCodes.Nop.ToInstruction();
            code.Add(OpCodes.Nop.ToInstruction());
            code.Add(branchTarget);
            code.Add(OpCodes.Br.ToInstruction(branchTarget));
            var module = CreateModule();
            var method = BuildMethod(code, module);
            var basicBlockAnalyser = new BasicBlockAnalyser(method);
            var offsetToIndexMap = new Dictionary<int, int>();
            var ehClauses = new List<EHClause>();

            var basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            Assert.That(basicBlocks.Count(x => x != null), Is.EqualTo(2));
            Assert.That(basicBlocks[branchTarget.Offset], Is.Not.Null);
        }
    }
}
