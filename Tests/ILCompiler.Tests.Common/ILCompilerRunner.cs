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

            _corelibPath = Path.Combine(solutionPath, $@".\System.Private.CoreLib\bin\Trs80\{buildConfigurationName}\net8.0\System.Private.CoreLib.dll");
        }

        public static ILCompilerRunner Create(string solutionPath)
        {
            return new ILCompilerRunner(solutionPath);
        }

        public void CompileILAndAssemble(string ilFileName, bool createLibrary = true, string optionalArguments = "")
        {
            CompileIL(ilFileName, createLibrary, optionalArguments);
        }

        private void CompileIL(string ilFileName, bool createLibrary, string optionalArguments)
        {
            var asmFileName = Path.ChangeExtension(ilFileName, "dasm");
            var exeFileName = Path.ChangeExtension(ilFileName, createLibrary ? "dll" : "exe");

            string arguments = $"-ao cim --ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath {_corelibPath} --outputFile {asmFileName} {exeFileName} --stackStart {StackStart}";

            if (!string.IsNullOrWhiteSpace(optionalArguments))
            {
                arguments += " " + optionalArguments;
            }

            var compiled = ILCompiler.Program.Main(arguments.Split(' '));

            Assert.AreEqual(0, compiled, "IL Failed to compile");
        }
    }
}
