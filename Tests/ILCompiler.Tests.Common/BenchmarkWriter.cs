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

        private static void RetryFileOperation(Action fileAction, int maxRetries = 3, int delayMilliseconds = 1000)
        {
            int attempt = 0;
            while (attempt < maxRetries)
            {
                try
                {
                    fileAction();
                    return;
                }
                catch (IOException ex)
                {
                    attempt++;
                    if (attempt >= maxRetries)
                    {
                        throw;
                    }
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delayMilliseconds}ms...");
                    Thread.Sleep(delayMilliseconds);
                }
            }
        }

        public void WriteBenchmark(string testName, ulong tStates)
        {
            RetryFileOperation(() => { WriteBenchmarkOperation(testName, tStates); });
        }

        private void WriteBenchmarkOperation(string testName, ulong tStates)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var benchmarks = new List<BenchmarkData>();

            using (var stream = new FileStream(_benchmarkResultsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                using (var reader = new StreamReader(stream, leaveOpen: true))
                {
                    var currentJson = reader.ReadToEnd();
                    if (currentJson.Length > 0)
                    {
                        benchmarks = JsonSerializer.Deserialize<List<BenchmarkData>>(currentJson, serializeOptions);

                        if (benchmarks == null)
                        {
                            throw new NullReferenceException($"Deserialization of benchmark file {_benchmarkResultsPath} failed");
                        }
                    }
                }

                benchmarks.Add(new BenchmarkData() { Name = testName, Unit = "T-States", Value = (int)tStates });

                var updatedJson = JsonSerializer.Serialize<List<BenchmarkData>>(benchmarks, serializeOptions);

                stream.SetLength(0);
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(updatedJson);
                }
            }
        }
    }
}
