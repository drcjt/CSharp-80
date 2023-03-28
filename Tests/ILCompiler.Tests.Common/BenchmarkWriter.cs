using System.Text.Json;

namespace ILCompiler.Tests.Common
{
    internal class BenchmarkWriter
    {
        private class BenchmarkData
        {
            public string Name { get; set; } = "Undefined";
            public string Unit { get; set; } = "Undefined";
            public int Value { get; set; }
        }

        private readonly string _benchmarkResultsPath;

        public BenchmarkWriter(string solutionPath)
        { 
            _benchmarkResultsPath = Path.Combine(solutionPath, "benchmark-results.txt");
        }

        public void WriteBenchmark(string testName, ulong tStates)
        {
            var benchmarks = new List<BenchmarkData>();
            if (File.Exists(_benchmarkResultsPath))
            {
                using (FileStream benchmarkFile = File.OpenRead(_benchmarkResultsPath)) 
                {
                    benchmarks = JsonSerializer.Deserialize<List<BenchmarkData>>(benchmarkFile);
                }
            }
            
            if (benchmarks == null)
            {
                throw new NullReferenceException($"Deserialization of benchmark file {_benchmarkResultsPath} failed");
            }

            benchmarks.Add(new BenchmarkData() { Name = testName, Unit = "T-States", Value = (int)tStates });

            var json = JsonSerializer.Serialize<List<BenchmarkData>>(benchmarks);

            File.WriteAllText(_benchmarkResultsPath, json);
        }
    }
}
