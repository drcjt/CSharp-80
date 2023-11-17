using ILCompiler.Interfaces;
using System.Text;

namespace ILCompiler.Compiler.Z80Assembler
{
    public class Z80Assembler : IZ80Assembler
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
            if (string.IsNullOrEmpty(_configuration.AssemblerOutput))
            {
                // Determine output type based on the target architecture
                outputTypes.Add(GetOutputType(_configuration.TargetArchitecture));
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

            var builder = new StringBuilder();
            builder.Append($"{_configuration.AssemblerArguments} --oo {string.Join(",", outputTypes)}");
            foreach (var outputPath in outputPaths)
            {
                builder.Append($" -o {outputPath}");
            }
            builder.Append($" {Path.GetFullPath(assemblyFileName)}");
            return builder.ToString();
        }

        private static string GetOutputType(TargetArchitecture targetArchitecture) 
        {
            return targetArchitecture switch
            {
                TargetArchitecture.TRS80 => "cmd",
                TargetArchitecture.ZXSpectrum => "tap",
                TargetArchitecture.CPM => "hex",
                _ => throw new ArgumentOutOfRangeException(nameof(targetArchitecture), $"Invalid target architecture value {targetArchitecture}"),
            };
        }

        private static void DownloadAssembler(string zmacPath)
        {
            var tempPath = Path.GetTempFileName();

            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync(ZmacUrl))
                {
                    using (var fs = new FileStream(tempPath, FileMode.OpenOrCreate))
                    {
                        s.Result.CopyTo(fs);
                    }
                }
            }

            try
            {
                File.Move(tempPath, zmacPath);
            }
            catch
            {
                // Ignore errors as another ILCompiler process running at same time may
                // have downloaded the compiler already
            }
        }
    }
}
