namespace ILCompiler.Tests.Common
{
    internal class BenchmarkWriter
    {
        private string _benchmarkResultsPath;

        public BenchmarkWriter(string solutionPath)
        { 
            _benchmarkResultsPath = Path.Combine(solutionPath, "benchmark-results.txt");
        }

        public void WriteBenchmark(string testName, ulong tStates)
        {
            using (StreamWriter sw = File.AppendText(_benchmarkResultsPath))
            {
                sw.WriteLine($"{testName}, {tStates}, {DateTime.Now}");
            }
        }
    }
}
