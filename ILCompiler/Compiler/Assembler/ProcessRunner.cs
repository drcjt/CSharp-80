using System.Diagnostics;

namespace ILCompiler.Compiler.Z80Assembler
{
    internal class ProcessRunner
    {
        internal static bool RunProcess(string filename, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = filename;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    var errors = process.StandardError.ReadToEnd();

                    Console.WriteLine($"Process failed");
                    Console.WriteLine(output);
                    Console.WriteLine(errors);

                    return false;
                }
            }

            return true;
        }
    }
}
