using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ILCompiler.UnitTests
{
    [TestFixture]
    public class InstanceFieldLayoutTests
    {
        private ModuleDefMD _testModule;

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..");

        [SetUp]
        public void Setup()
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();

            var currentType = MethodBase.GetCurrentMethod().DeclaringType;
            var assemblyConfigurationAttribute = currentType.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;
           
            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\net6.0\System.Private.CoreLib.dll");

            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);
            var corlibAssemblyRef = corlibModule.Assembly.ToAssemblyRef();

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibAssemblyRef
            };

            string inputFilePath = Path.Combine(SolutionPath, $@".\Tests\CoreTestAssembly\bin\{buildConfigurationName}\net6.0\CoreTestAssembly.dll");
            _testModule = ModuleDefMD.Load(inputFilePath, options);
        }

        [Test]
        public void TestSequentialTypeLayout()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Struct0", false);

            var target = new TargetDetails(TargetArchitecture.Z80);
            var metadataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDef);

            foreach (var fieldAndOffset in computedFieldLayout.Offsets)
            {
                var field = fieldAndOffset.Field;
                if (field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "b1":
                        Assert.AreEqual(0, fieldAndOffset.Offset.AsInt);
                        break;
                    case "b2":
                        Assert.AreEqual(1, fieldAndOffset.Offset.AsInt);
                        break;
                    case "b3":
                        Assert.AreEqual(2, fieldAndOffset.Offset.AsInt);
                        break;
                    case "i1":
                        Assert.AreEqual(4, fieldAndOffset.Offset.AsInt);
                        break;
                    case "s1":
                        Assert.AreEqual(8, fieldAndOffset.Offset.AsInt);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}   