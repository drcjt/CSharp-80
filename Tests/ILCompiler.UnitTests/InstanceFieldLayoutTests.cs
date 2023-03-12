using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
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

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\net7.0\System.Private.CoreLib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);
            ((AssemblyResolver)modCtx.AssemblyResolver).AddToCache(corlibModule);

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibModule.Assembly.ToAssemblyRef()
            };
            string inputFilePath = Path.Combine(SolutionPath, $@".\Tests\CoreTestAssembly\bin\{buildConfigurationName}\net7.0\CoreTestAssembly.dll");
            _testModule = ModuleDefMD.Load(inputFilePath, options);
        }


        [Test]
        public void TestSequentialTypeLayout()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Class1", false);

            var target = new TargetDetails(TargetArchitecture.Z80);
            var metadataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var computedFieldLayout = typeDef.InstanceFieldLayout(metadataFieldLayoutAlgorithm);

            // Byte count
            // Base Class       2 + 2 padding
            // MyInt            4
            // MyBool           1 + 1 padding
            // MyChar           2
            // MyString         2
            // MyByteArray      2
            // MyClass1SelfRef  2
            // -------------------
            //                  18 

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.AreEqual(18, instanceByteCount.AsInt);

            foreach (var fieldAndOffset in computedFieldLayout.Offsets)
            {
                var field = fieldAndOffset.Field;
                if (field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "MyInt":
                        Assert.AreEqual(4, fieldAndOffset.Offset.AsInt);
                        break;
                    case "MyBool":
                        Assert.AreEqual(8, fieldAndOffset.Offset.AsInt);
                        break;
                    case "MyChar":
                        Assert.AreEqual(10, fieldAndOffset.Offset.AsInt);
                        break;
                    case "MyString":
                        Assert.AreEqual(12, fieldAndOffset.Offset.AsInt);
                        break;
                    case "MyByteArray":
                        Assert.AreEqual(14, fieldAndOffset.Offset.AsInt);
                        break;
                    case "MyClass1SelfRef":
                        Assert.AreEqual(16, fieldAndOffset.Offset.AsInt);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestSequentialTypeLayoutStruct()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Struct0", false);

            var target = new TargetDetails(TargetArchitecture.Z80);
            var metadataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var computedFieldLayout = typeDef.InstanceFieldLayout(metadataFieldLayoutAlgorithm);

            // Byte count
            // bool     b1      1
            // bool     b2      1
            // bool     b3      1 + 1 padding for int alignment
            // int      i1      4
            // string   s1      2 + 2 padding for int alignment
            // -------------------
            //                  12

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.AreEqual(12, instanceByteCount.AsInt);

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

        [Test]
        public void TestSequentialTypeLayoutStructEmbedded()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Struct1", false);

            var target = new TargetDetails(TargetArchitecture.Z80);
            var metadataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var computedFieldLayout = typeDef.InstanceFieldLayout(metadataFieldLayoutAlgorithm);

            // Byte count
            // struct   MyStruct0   12
            // bool     MyBool      1 + 3 for int aligment
            // -----------------------
            //                      16

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.AreEqual(16, instanceByteCount.AsInt);

            foreach (var fieldAndOffset in computedFieldLayout.Offsets)
            {
                var field = fieldAndOffset.Field;
                if (field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "MyStruct0":
                        Assert.AreEqual(0, fieldAndOffset.Offset.AsInt);
                        break;
                    case "MyBool":
                        Assert.AreEqual(12, fieldAndOffset.Offset.AsInt);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}