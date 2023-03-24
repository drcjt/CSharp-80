namespace ILCompiler.Tests.Common
{
    internal class Assembler
    {
        private string _zmacPath;

        public Assembler(string solutionPath)
        {
            _zmacPath = Path.Combine(solutionPath, @".\tools\zmac.exe");
        }

        public static Assembler Create(string solutionPath)
        {
            return new Assembler(solutionPath);
        }

        public void Assemble(string ilFileName)
        {
            var asmFileName = Path.ChangeExtension(ilFileName, "asm");
            var cimFileName = Path.ChangeExtension(ilFileName, "cim");

            var arguments = $"--oo cim -o {cimFileName} {asmFileName}";

            ProcessRunner.RunProcess(_zmacPath, arguments);
        }
    }
}
