using System.Globalization;
using System.Text.RegularExpressions;

namespace JitDiff
{
    internal partial class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: jitdiff base diff");
                return 0;
            }

            var basePath = args[0];
            var diffPath = args[1];

            var baseList = ExtractFileInfo(basePath, ".dasm");
            var diffList = ExtractFileInfo(diffPath, ".dasm");

            var compareList = Comparator(baseList, diffList);

            return Summarize(compareList);
        }

        private sealed record MethodInfo(string Name, int CodeSize);
        private sealed record MethodDelta(string Name, int BaseBytes, int DiffBytes)
        {
            public int DeltaBytes => DiffBytes - BaseBytes;
        }

        private sealed class MethodInfoComparer : IEqualityComparer<MethodInfo>
        {
            public bool Equals(MethodInfo? x, MethodInfo? y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(MethodInfo methodInfo)
            {
                if (methodInfo is null) return 0;
                return methodInfo.Name == null ? 0 : methodInfo.Name.GetHashCode();
            }
        }

        private sealed record FileInfo(string Name, IEnumerable<MethodInfo> Methods);

        private sealed class FileDelta
        {
            public required string BaseName;
            public required string DiffName;
            public int DeltaBytes;
            public int MethodsInBoth;
            public required IEnumerable<MethodInfo> MethodsOnlyInBase;
            public required IEnumerable<MethodInfo> MethodsOnlyInDiff;
            public required IEnumerable<MethodDelta> MethodDeltas;
        }

        [GeneratedRegex(@"for method (.*)$")]
        private static partial Regex NamePattern();

        [GeneratedRegex(@"code ([0-9]{1,})")]
        private static partial Regex CodeSizePattern();

        private static IEnumerable<MethodInfo> ExtractMethodInfo(string filePath)
        {
            var result = File.ReadLines(filePath).Select((x, i) => new { line = x, index = i })
                .Where(l => l.line.StartsWith(@";Assembly listing for method", StringComparison.Ordinal)
                            || l.line.StartsWith(@";Total bytes of code", StringComparison.Ordinal))
                .Select((x) =>
                {
                    var nameMatch = NamePattern().Match(x.line);
                    var codeSize = CodeSizePattern().Match(x.line);

                    return new
                    {
                        name = nameMatch.Groups[1].Value,
                        totalBytes = codeSize.Success ? int.Parse(codeSize.Groups[1].Value, CultureInfo.InvariantCulture) : 0,
                    };
                })
                .GroupBy(x => x.name)
                .Select(x => new MethodInfo(x.Key, x.Sum(z => z.totalBytes))).ToList();

            return result;
        }

        private static List<FileInfo> ExtractFileInfo(string path, string fileExtension)
        {
            var searchOption = SearchOption.AllDirectories;
            var fullRootPath = Path.GetFullPath(path);

            if (Directory.Exists(fullRootPath))
            {
                var searchPattern = $"*{fileExtension}";
                return Directory.EnumerateFiles(fullRootPath, searchPattern, searchOption).
                    AsParallel().Select(p => new FileInfo(p.Substring(fullRootPath.Length).TrimStart(Path.DirectorySeparatorChar), ExtractMethodInfo(p))).ToList();
            }
            else
            {
                return [new FileInfo(Path.GetFileName(path), ExtractMethodInfo(path))];
            }
        }

        private static IEnumerable<FileDelta> Comparator(IEnumerable<FileInfo> baseInfo, IEnumerable<FileInfo> diffInfo)
        {
            var methodInfoComparer = new MethodInfoComparer();

            return baseInfo.Join(diffInfo, b => b.Name, d => d.Name, (b, d) =>
            {
                var jointList = b.Methods.Join(d.Methods,
                    x => x.Name, y => y.Name, (x, y) => new MethodDelta(x.Name, x.CodeSize, y.CodeSize))
                    .Where(r => r.DeltaBytes != 0)
                    .OrderByDescending(r => r.DeltaBytes);

                return new FileDelta
                {
                    BaseName = b.Name,
                    DiffName = d.Name,
                    DeltaBytes = jointList.Sum(x => x.DeltaBytes),
                    MethodsInBoth = jointList.Count(),
                    MethodsOnlyInBase = b.Methods.Except(d.Methods, methodInfoComparer),
                    MethodsOnlyInDiff = d.Methods.Except(b.Methods, methodInfoComparer),
                    MethodDeltas = jointList,
                };
            }).ToList();
        }

        private static int Summarize(IEnumerable<FileDelta> fileDeltas)
        {
            var totalBytes = fileDeltas.Sum(x => x.DeltaBytes);

            Console.WriteLine("\nSummary:\n(Note: Lower is better)\n");
            Console.WriteLine($"Total bytes of diff: {totalBytes}");

            if (totalBytes != 0)
            {
                Console.WriteLine("\tdiff is {0}.", totalBytes < 0 ? "an improvement" : "a regression");
            }

            var sortedDeltas = fileDeltas.Where(x => x.DeltaBytes != 0).OrderByDescending(d => d.DeltaBytes).ToList();

            var sortedFileCount = sortedDeltas.Count;
            int fileCount = sortedFileCount < 5 ? sortedFileCount : 5;

            if (sortedFileCount> 0 && sortedDeltas[0].DeltaBytes > 0)
            {
                Console.WriteLine("\nTop file regressions by size (bytes).");
                foreach (var fileDelta in sortedDeltas.GetRange(0, fileCount).Where(x => x.DeltaBytes > 0)) 
                {
                    Console.WriteLine($"\t{fileDelta.DeltaBytes,8} : {fileDelta.BaseName}");
                }
            }

            return Math.Abs(totalBytes);
        }
    }
}
