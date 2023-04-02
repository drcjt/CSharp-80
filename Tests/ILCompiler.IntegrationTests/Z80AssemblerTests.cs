using ILCompiler.Compiler;
using ILCompiler.Compiler.Z80Assembler;
using ILCompiler.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.IO;

namespace ILCompiler.IntegrationTests
{
    [TestFixture]
    internal class Z80AssemblerTests
    {
        [Test]
        public void Assemble_WhenZmacInLocalAppData_DownloadsZmac()
        {
            var mockConfiguration = new Mock<IConfiguration>();
            var assembler = new Z80Assembler(mockConfiguration.Object);

            // Delete zmac file if it exists
            var ilCompilerApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ILCompiler");
            var zmacPath = Path.Combine(ilCompilerApplicationDirectory, "zmac.exe");
            File.Delete(zmacPath);

            assembler.Assemble("testAssembly.asm");

            // Validate that zmac has been downloaded
            Assert.IsTrue(File.Exists(zmacPath));
        }

        [TestCase(TargetArchitecture.TRS80, "cmd")]
        [TestCase(TargetArchitecture.ZXSpectrum, "tap")]
        [TestCase(TargetArchitecture.CPM, "hex")]
        public void Assemble_WithTargetArchitecture_CreatesArchitectureSpecificOutputFile(TargetArchitecture targetArchitecture, string expectedOutputType)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x.TargetArchitecture).Returns(targetArchitecture);
            var assembler = new Z80Assembler(mockConfiguration.Object);

            // Create empty assembly file
            var assemblyFile = Path.GetTempFileName();
            File.WriteAllText(assemblyFile, "");

            // Ensure expected output file doesn't exist
            var expectedOutputFile = Path.ChangeExtension(assemblyFile, expectedOutputType);
            File.Delete(expectedOutputFile);

            assembler.Assemble(assemblyFile);

            // Validate that the expected output file has been created by the assembler
            Assert.IsTrue(File.Exists(expectedOutputFile));
        }
    }
}
