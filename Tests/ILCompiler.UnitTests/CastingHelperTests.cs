using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ILCompiler.UnitTests
{
    [TestFixture]
    internal class CastingHelperTests
    {
        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..");

        private TypeSystemContext context;

        [SetUp]
        public void Setup()
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();

            var currentType = MethodBase.GetCurrentMethod().DeclaringType;
            var assemblyConfigurationAttribute = currentType.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\System.Private.CoreLib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);

            context = new TypeSystemContext(new Configuration());
            var corLibModuleProvider = new CorLibModuleProvider
            {
                CorLibModule = corlibModule
            };
            ModuleDesc module = new DnlibModule(context, corLibModuleProvider, null);
            context.SetSystemModule(module);
        }

        [Test]
        public void IsEquivalentTo_ForArrayAndNonArrayType_ReturnsFalse()
        {
            var int32Type = context.GetWellKnownType(WellKnownType.Int32);

            var arrayType = context.GetArrayType(int32Type);
            Assert.That(CastingHelper.IsEquivalentTo(arrayType, int32Type), Is.Not.True);
        }

        [Test]
        public void IsEquivalentTo_ForDefTypeAndNonDefType_ReturnsFalse()
        {
            var int32Type = context.GetWellKnownType(WellKnownType.Int32);
            var arrayType = context.GetArrayType(int32Type);

            Assert.That(CastingHelper.IsEquivalentTo(int32Type, arrayType), Is.Not.True);
        }
    }
}
