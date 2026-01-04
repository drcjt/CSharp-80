using NUnit.Framework;
using System.Reflection;

namespace ILCompiler.Tests.Common
{
    public class ILCompilerRunner
    {
        public const int StackStart = UInt16.MaxValue;

        private readonly string _corelibPath;

        public ILCompilerRunner(string solutionPath)
        {
            var currentType = MethodBase.GetCurrentMethod()?.DeclaringType;
            var assemblyConfigurationAttribute = currentType?.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

            _corelibPath = Path.Combine(solutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\System.Private.CoreLib.dll");
        }

        public static ILCompilerRunner Create(string solutionPath)
        {
            return new ILCompilerRunner(solutionPath);
        }

        public void CompileILAndAssemble(string ilFileName, bool createLibrary = true, string optionalArguments = "", bool setCurrentDirectory = true)
        {
            CompileIL(ilFileName, createLibrary, optionalArguments, setCurrentDirectory);
        }

        private void CompileIL(string ilFileName, bool createLibrary, string optionalArguments, bool setCurrentDirectory)
        {
            var asmFileName = Path.ChangeExtension(ilFileName, "dasm");
            var exeFileName = Path.ChangeExtension(ilFileName, createLibrary ? "dll" : "exe");

            string arguments = $"-ao cim --ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath {_corelibPath} --outputFile {asmFileName} {exeFileName} --stackStart {StackStart}";

            if (!string.IsNullOrWhiteSpace(optionalArguments))
            {
                arguments += " " + optionalArguments;
            }

            if (setCurrentDirectory)
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(ilFileName)!);
            }

            int compiled = 0;
            try
            {
                compiled = Program.Main(arguments.Split(' '));
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to compile IL: {ex.Message}");
            }

            Assert.That(compiled, Is.EqualTo(0), "IL Failed to compile");
        }
    }
}
