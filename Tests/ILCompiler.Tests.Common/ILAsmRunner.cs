namespace ILCompiler.Tests.Common
{
    public class ILAsmRunner
    {
        public static bool Assemble(string ilFileName)
        {
            var ilAsmPath = @"%USERPROFILE%\.nuget\packages\runtime.win-x64.microsoft.netcore.ilasm\8.0.0\runtimes\win-x64\native\ilasm.exe";
            ilAsmPath = Environment.ExpandEnvironmentVariables(ilAsmPath);

            return ProcessRunner.RunProcess(ilAsmPath, ilFileName);
        }
    }
}
