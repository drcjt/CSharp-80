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

            _corelibPath = Path.Combine(solutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\net7.0\System.Private.CoreLib.dll");
        }

        public static ILCompilerRunner Create(string solutionPath)
        {
            return new ILCompilerRunner(solutionPath);
        }

        public void CompileILAndAssemble(string ilFileName, bool createLibrary = true)
        {
            CompileIL(ilFileName, createLibrary);
        }

        public void CompileIL(string ilFileName, bool createLibrary = true)
        {
            var asmFileName = Path.ChangeExtension(ilFileName, "asm");
            var exeFileName = Path.ChangeExtension(ilFileName, createLibrary ? "dll" : "exe");

            var arguments = $"-ao cim --ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath {_corelibPath} --outputFile {asmFileName} {exeFileName} --stackStart {StackStart}";

            var compiled = ILCompiler.Program.Main(arguments.Split(' '));

            Assert.AreEqual(0, compiled, "IL Failed to compile");
        }
    }
}
