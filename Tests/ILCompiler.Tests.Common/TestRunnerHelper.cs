using NUnit.Framework;
using System.Diagnostics;

namespace ILCompiler.Tests.Common
{
    public static class TestRunnerHelper
    {
        private const string IlExtension = ".il";
        private const string CimExtension = ".cim";
        private const string ProjectFileExtension = "*.csproj";

        public static void RunTest(string testName, string path)
        {
            var solutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, path);

            // Determine if test is using il file or not
            if (Path.GetExtension(testName) == IlExtension)
            {
                // Assemble the il
                if (!ILAsmRunner.Assemble(testName))
                {
                    Assert.Fail("Failed to assemble IL");
                }

                var cimFile = Path.ChangeExtension(testName, CimExtension);

                // Run the test
                ILCompilerRunner.Create(solutionPath).CompileILAndAssemble(cimFile, createLibrary: false);
                Z80TestRunner.Create(solutionPath).RunTest(cimFile);
            }
            else
            {
                ILCompilerRunner.Create(solutionPath).CompileILAndAssemble(testName);
                Z80TestRunner.Create(solutionPath).RunTest(testName);
            }
        }

        private static List<string> GetFilesIncludingSubfolders(string path, string searchPattern)
        {
            var paths = new List<string>();
            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                paths.AddRange(GetFilesIncludingSubfolders(directory, searchPattern));
            }
            paths.AddRange([.. Directory.GetFiles(path, searchPattern)]);
            return paths;
        }

        public static bool IsTestRunner(string projectFile) => File.ReadAllText(projectFile).Contains("ILCompiler.Tests.Common");

        public static IEnumerable<TestCaseData> TestsCaseData()
        {
            var testsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
            var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);

            var projectFiles = GetFilesIncludingSubfolders(testsPath, ProjectFileExtension);
            foreach (var projectFile in projectFiles)
            {
                var testPath = Path.GetDirectoryName(projectFile);
                var testName = Path.GetFileNameWithoutExtension(projectFile);

                Debug.Assert(testPath != null);

                if (!IsTestRunner(projectFile))
                {
                    var ilFiles = Directory.GetFiles(testPath, "*.il");

                    if (ilFiles.Length == 0)
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{testName}{CimExtension}");
                        var testAssemblyPath = Path.Combine(testPath, temp);

                        // Group by parent folders
                        var relativePath = testPath.Replace(testsPath, string.Empty).TrimStart(Path.DirectorySeparatorChar);
                        relativePath = Path.GetDirectoryName(relativePath);

                        if (!string.IsNullOrEmpty(relativePath))
                        {
                            testName = $"{relativePath} ({testName})";
                        }

                        // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                        testName = testName.Replace('.', '\u2024');

                        yield return new TestCaseData(testAssemblyPath).SetName(testName);
                    }
                    else
                    {
                        foreach (var ilFile in ilFiles)
                        {
                            var ilFileName = Path.GetFileName(ilFile);
                            var temp = Path.Combine(binConfigTargetPath, ilFileName);
                            var testAssemblyPath = Path.Combine(testPath, temp);

                            // Group by parent folders
                            var relativePath = testPath.Replace(testsPath, string.Empty).TrimStart(Path.DirectorySeparatorChar);
                            relativePath = Path.GetDirectoryName(relativePath) ?? "";
                            if (string.IsNullOrEmpty(relativePath))
                            {
                                relativePath += testName;
                            }
                            else
                            {
                                relativePath += $"{Path.DirectorySeparatorChar}{testName}";
                            }

                            testName = $"{relativePath} ({Path.GetFileNameWithoutExtension(ilFile)})";

                            // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                            testName = testName.Replace('.', '\u2024');

                            yield return new TestCaseData(testAssemblyPath).SetName(testName);
                        }
                    }
                }
            }
        }
    }
}
