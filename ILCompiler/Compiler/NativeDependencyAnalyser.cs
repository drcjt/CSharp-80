using ILCompiler.Interfaces;
using System.Text.RegularExpressions;

namespace ILCompiler.Compiler
{
    public class NativeDependencyAnalyser
    {
        private readonly IDictionary<string, string> _labelMap = new Dictionary<string, string>();
        private readonly IDictionary<string, ISet<string>> _callsMap = new Dictionary<string, ISet<string>>();

        private readonly IConfiguration _configuration;
        public NativeDependencyAnalyser(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }  

        public IEnumerable<string> GetNativeResourceNames(ISet<string> calls)
        {
            BuildNativeLabelMap();
            BuildCallsMap();

            var topLevelAsmResourceNames = new HashSet<string>();
            foreach (var call in calls)
            {
                if (_labelMap.TryGetValue(call, out var asmResourceName))
                {
                    topLevelAsmResourceNames.Add(asmResourceName);
                }
            }

            var dependencies = GetNativeDependencies(topLevelAsmResourceNames);
            dependencies.UnionWith(topLevelAsmResourceNames);
            return dependencies;
        }

        private ISet<string> GetNativeDependencies(ISet<string> resourceNames)
        {
            var dependencies = new HashSet<string>();

            foreach (var resourceName in resourceNames)
            {
                var calls = _callsMap[resourceName];
                foreach (var call in calls)
                {
                    if (_labelMap.ContainsKey(call))
                    {
                        if (_labelMap[call] != resourceName)
                        {
                            dependencies.Add(_labelMap[call]);
                        }
                    }
                }
            }

            if (dependencies.Count > 0)
            {
                var indirectDependencies = GetNativeDependencies(dependencies);
                dependencies.UnionWith(indirectDependencies);
            }

            return dependencies;
        }

        public static ISet<string> GetMethodCalls(string methodCode) => GetCalls(new StringReader(methodCode));

        private static ISet<string> GetCalls(TextReader reader)
        {
            var calls = new HashSet<string>();
            var callRegex = new Regex(@"^\s*(call|jp)\s*(.*\s*,\s*)?(?<target>\w+)", RegexOptions.IgnoreCase);

            var line = reader.ReadLine();
            while (line != null)
            {
                var match = callRegex.Match(line);
                if (match.Success)
                {
                    calls.Add(match.Groups["target"].Value.ToUpper());
                }
                line = reader.ReadLine();
            }

            return calls;
        }

        private void BuildCallsMap()
        {
            foreach (var nativeResource in GetNativeResourceNames())
            {
                using Stream? stream = GetType().Assembly.GetManifestResourceStream(nativeResource);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    _callsMap[nativeResource] = GetCalls(reader);
                }
            }
        }

        private void BuildNativeLabelMap()
        {
            var labelRegex = new Regex(@"^\s*(?<label>\w+)\s*:");

            foreach (var resourceName in GetNativeResourceNames())
            {
                using Stream? stream = GetType().Assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        var match = labelRegex.Match(line);
                        if (match.Success)
                        {
                            _labelMap.Add(match.Groups["label"].Value.ToUpper(), resourceName);
                        }
                        line = reader.ReadLine();
                    }
                }
            }
        }

        private IEnumerable<string> GetNativeResourceNames()
        {
            var filteredResourceNames = new List<string>();
            string[] resourceNames = GetType().Assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.StartsWith("ILCompiler.Runtime.TRS80") && _configuration.TargetArchitecture != TargetArchitecture.TRS80) continue;
                if (resourceName.StartsWith("ILCompiler.Runtime.CPM") && _configuration.TargetArchitecture != TargetArchitecture.CPM) continue;
                if (resourceName.StartsWith("ILCompiler.Runtime.ZXSpectrum") && _configuration.TargetArchitecture != TargetArchitecture.ZXSpectrum) continue;

                filteredResourceNames.Add(resourceName);
            }

            return filteredResourceNames;
        }
    }
}
