namespace ILCompiler.Tests.Common
{
    public class ILAsmRunner
    {
        public static void Assemble(string ilFileName)
        {
            var ilAsmPath = @"%USERPROFILE%\.nuget\packages\microsoft.netcore.ilasm\6.0.0\runtimes\native\ilasm.exe";
            ilAsmPath = Environment.ExpandEnvironmentVariables(ilAsmPath);

            ProcessRunner.RunProcess(ilAsmPath, ilFileName);
        }
    }
}
