using NUnit.Framework;
using System.Reflection;

namespace ILCompiler.Tests.Common
{
    public class ILCompilerRunner
    {
        public const int StackStart = UInt16.MaxValue;
        private const string ZmacExe = "zmac.exe";

        private readonly string _corelibPath;

        public ILCompilerRunner(string solutionPath)
        {
            var currentType = MethodBase.GetCurrentMethod()?.DeclaringType;
            var assemblyConfigurationAttribute = currentType?.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

            _corelibPath = Path.Combine(solutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\net7.0\System.Private.CoreLib.dll");
        }

        public static ILCompilerRunner Create(string solutionPath)
        {
            return new ILCompilerRunner(solutionPath);
        }

        public void CompileILAndAssemble(string ilFileName, bool createLibrary = true, bool outputCim = true, bool deleteAssembler = false)
        {
            // If test requires assembler to not already have been downloaded then delete it first
            if (deleteAssembler)
            {
                var ilCompilerApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ILCompiler");
                var zmacPath = Path.Combine(ilCompilerApplicationDirectory, ZmacExe);
                if (File.Exists(zmacPath))
                {
                    File.Delete(zmacPath);
                }
            }

            CompileIL(ilFileName, createLibrary, outputCim);
        }

        private void CompileIL(string ilFileName, bool createLibrary, bool outputCim)
        {
            var asmFileName = Path.ChangeExtension(ilFileName, "asm");
            var exeFileName = Path.ChangeExtension(ilFileName, createLibrary ? "dll" : "exe");

            var arguments = outputCim ? "-ao cim " : "";
            arguments += $"--ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath {_corelibPath} --outputFile {asmFileName} {exeFileName} --stackStart {StackStart}";

            var compiled = ILCompiler.Program.Main(arguments.Split(' '));

            Assert.AreEqual(0, compiled, "IL Failed to compile");
        }
    }
}
