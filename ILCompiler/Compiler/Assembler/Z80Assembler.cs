using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Z80Assembler
{
    internal class Z80Assembler : IZ80Assembler
    {
        private readonly IConfiguration _configuration;
        private const string ZmacUrl = "https://github.com/gp48k/zmac/raw/master/zmac.exe";
        private const string ZmacExe = "zmac.exe";

        public Z80Assembler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Assemble(string assemblyFileName)
        {
            if (_configuration.NoAssemble)
            {
                return;
            }

            var ilCompilerApplicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ILCompiler");
            var zmacPath = Path.Combine(ilCompilerApplicationDirectory, ZmacExe);

            if (!Directory.Exists(ilCompilerApplicationDirectory))
            {
                Directory.CreateDirectory(ilCompilerApplicationDirectory);
            }

            if (!File.Exists(zmacPath))
            {
                DownloadAssembler(zmacPath);
            }

            string arguments = BuildArguments(assemblyFileName);

            // Assemble the specified file
            ProcessRunner.RunProcess(zmacPath, arguments);
        }

        private string BuildArguments(string assemblyFileName)
        {
            var outputTypes = new List<string>();
            if (_configuration.AssemblerOutput == string.Empty)
            {
                // Determine output type based on the target architecture
                outputTypes.Add(_configuration.TargetArchitecture switch
                {
                    TargetArchitecture.TRS80 => "cmd",
                    TargetArchitecture.ZXSpectrum => "tap",
                    TargetArchitecture.CPM => "hex",
                    _ => throw new ArgumentOutOfRangeException(nameof(TargetArchitecture), $"Unknown Target Architecture {_configuration.TargetArchitecture}")
                });
            }
            else
            {
                outputTypes.Add(_configuration.AssemblerOutput);
            }

            if (!_configuration.NoListFile)
            {
                outputTypes.Add("lst");
            }

            var outputPaths = new List<string>();
            foreach (var outputType in outputTypes)
            {
                outputPaths.Add(Path.ChangeExtension(assemblyFileName, outputType));
            }

            var arguments = $"{_configuration.AssemblerArguments} --oo {string.Join(",", outputTypes)}";
            foreach (var outputPath in outputPaths)
            {
                arguments += $" -o {outputPath}";
            }
            arguments += $" {Path.GetFullPath(assemblyFileName)}";
            return arguments;
        }

        private static void DownloadAssembler(string zmacPath)
        {
            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync(ZmacUrl))
                {
                    using (var fs = new FileStream(zmacPath, FileMode.OpenOrCreate))
                    {
                        s.Result.CopyTo(fs);
                    }
                }
            }
        }
    }
}
