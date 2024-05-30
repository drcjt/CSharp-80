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

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\Trs80\{buildConfigurationName}\net8.0\System.Private.CoreLib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);
            ((AssemblyResolver)modCtx.AssemblyResolver).AddToCache(corlibModule);

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibModule.Assembly.ToAssemblyRef()
            };
            string inputFilePath = Path.Combine(SolutionPath, $@".\Tests\CoreTestAssembly\bin\{buildConfigurationName}\net8.0\CoreTestAssembly.dll");
            _testModule = ModuleDefMD.Load(inputFilePath, options);
        }

        [Test]
        public void TestSequentialTypeLayout_WithFixedSizeBuffer()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Struct2", false);

            var target = new TargetDetails(TargetArchitecture.Z80);

            var typeSystemContext = new TypeSystemContext(null);
            var typeDesc = typeSystemContext.Create(typeDef);
            var newMetaDataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);
            var computedFieldLayout = newMetaDataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

            // Byte count
            // bool     b1           1 + 1 padding
            // char[25] fixedBuffer  50
            // int      i1           4
            // ------------------------
            //                       56

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.AreEqual(56, instanceByteCount.AsInt);

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "b1":
                        Assert.AreEqual(0, fieldOffset);
                        break;
                    case "fixedBuffer":
                        Assert.AreEqual(2, fieldOffset);
                        break;
                    case "i1":
                        Assert.AreEqual(52, fieldOffset);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }


        [Test]
        public void TestSequentialTypeLayout()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Class1", false);

            var target = new TargetDetails(TargetArchitecture.Z80);
            var metadataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var typeSystemContext = new TypeSystemContext(null);
            var typeDesc = typeSystemContext.Create(typeDef);
            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

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

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "MyInt":
                        Assert.AreEqual(4, fieldOffset);
                        break;
                    case "MyBool":
                        Assert.AreEqual(8, fieldOffset);
                        break;
                    case "MyChar":
                        Assert.AreEqual(10, fieldOffset);
                        break;
                    case "MyString":
                        Assert.AreEqual(12, fieldOffset);
                        break;
                    case "MyByteArray":
                        Assert.AreEqual(14, fieldOffset);
                        break;
                    case "MyClass1SelfRef":
                        Assert.AreEqual(16, fieldOffset);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestSequentialTypeLayoutInheritance()
        {
            var typeDef = _testModule.Find("CoreTestAssembly.Class2", false);

            var target = new TargetDetails(TargetArchitecture.Z80);
            var metadataFieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var typeSystemContext = new TypeSystemContext(null);
            var typeDesc = typeSystemContext.Create(typeDef);
            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

            // Byte count
            // Base Class       18
            // MyInt2           4 + 2 byte padding to make int field start on 4 byte alignment
            // -------------------
            //                  24

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.AreEqual(24, instanceByteCount.AsInt);

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "MyInt2":
                        Assert.AreEqual(20, fieldOffset);
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

            var typeSystemContext = new TypeSystemContext(null);
            var typeDesc = typeSystemContext.Create(typeDef);
            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

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

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "b1":
                        Assert.AreEqual(0, fieldOffset);
                        break;
                    case "b2":
                        Assert.AreEqual(1, fieldOffset);
                        break;
                    case "b3":
                        Assert.AreEqual(2, fieldOffset);
                        break;
                    case "i1":
                        Assert.AreEqual(4, fieldOffset);
                        break;
                    case "s1":
                        Assert.AreEqual(8, fieldOffset);
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

            var typeSystemContext = new TypeSystemContext(null);
            var typeDesc = typeSystemContext.Create(typeDef);
            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

            // Byte count
            // struct   MyStruct0   12
            // bool     MyBool      1 + 3 for int alignment
            // -----------------------
            //                      16

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.AreEqual(16, instanceByteCount.AsInt);

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "MyStruct0":
                        Assert.AreEqual(0, fieldOffset);
                        break;
                    case "MyBool":
                        Assert.AreEqual(12, fieldOffset);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        private int GetFieldOffset(FieldAndOffset[] fieldAndOffsets, string fieldName)
        {
            foreach (var fieldAndOffset in fieldAndOffsets)
            {
                if (fieldAndOffset.Field.Name == fieldName)
                {
                    return fieldAndOffset.Offset.AsInt;
                }
            }

            throw new InvalidDataException();
        }
    }
}