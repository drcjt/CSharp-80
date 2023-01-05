namespace RegressionTests
{
    internal static class PathExtensions
    {
        public static string RemoveDirectories(this string path, int parentCount)
        {
            var result = new DirectoryInfo(path);
            while (parentCount > 0 && result != null)
            {
                result = Directory.GetParent(result.FullName);
                parentCount--;
            }

            return result != null ? result.FullName : String.Empty;
        }

        public static string GetLastDirectories(this string path, int directoryCount)
        {
            var result = String.Empty;
            var currentDirectory = new DirectoryInfo(path);
            while (directoryCount > 0 && currentDirectory != null)
            {
                result = Path.Combine(currentDirectory.Name, result);
                currentDirectory = Directory.GetParent(currentDirectory.FullName);
                directoryCount--;
            }

            return result;
        }

    }
}
