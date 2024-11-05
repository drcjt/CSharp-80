using dnlib.DotNet;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
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

            var module = new DnlibModule(new TypeSystemContext(), new Compiler.CorLibModuleProvider(), null);
            var typeDesc = module.Create(typeDef);
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

            Assert.That(instanceByteCount.AsInt, Is.EqualTo(56));

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "b1":
                        Assert.That(fieldOffset, Is.EqualTo(0));
                        break;
                    case "fixedBuffer":
                        Assert.That(fieldOffset, Is.EqualTo(2));
                        break;
                    case "i1":
                        Assert.That(fieldOffset, Is.EqualTo(52));
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

            var module = new DnlibModule(new TypeSystemContext(), new Compiler.CorLibModuleProvider(), null);
            var typeDesc = module.Create(typeDef);
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

            Assert.That(instanceByteCount.AsInt, Is.EqualTo(18));

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "MyInt":
                        Assert.That(fieldOffset, Is.EqualTo(4));
                        break;
                    case "MyBool":
                        Assert.That(fieldOffset, Is.EqualTo(8));
                        break;
                    case "MyChar":
                        Assert.That(fieldOffset, Is.EqualTo(10));
                        break;
                    case "MyString":
                        Assert.That(fieldOffset, Is.EqualTo(12));
                        break;
                    case "MyByteArray":
                        Assert.That(fieldOffset, Is.EqualTo(14));
                        break;
                    case "MyClass1SelfRef":
                        Assert.That(fieldOffset, Is.EqualTo(16));
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

            var module = new DnlibModule(new TypeSystemContext(), new Compiler.CorLibModuleProvider(), null);
            var typeDesc = module.Create(typeDef);
            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

            // Byte count
            // Base Class       18
            // MyInt2           4 + 2 byte padding to make int field start on 4 byte alignment
            // -------------------
            //                  24

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.That(instanceByteCount.AsInt, Is.EqualTo(24));

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "MyInt2":
                        Assert.That(fieldOffset, Is.EqualTo(20));
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

            var module = new DnlibModule(new TypeSystemContext(), new Compiler.CorLibModuleProvider(), null);
            var typeDesc = module.Create(typeDef);
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

            Assert.That(instanceByteCount.AsInt, Is.EqualTo(12));

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "b1":
                        Assert.That(fieldOffset, Is.EqualTo(0));
                        break;
                    case "b2":
                        Assert.That(fieldOffset, Is.EqualTo(1));
                        break;
                    case "b3":
                        Assert.That(fieldOffset, Is.EqualTo(2));
                        break;
                    case "i1":
                        Assert.That(fieldOffset, Is.EqualTo(4));
                        break;
                    case "s1":
                        Assert.That(fieldOffset, Is.EqualTo(8));
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

            var module = new DnlibModule(new TypeSystemContext(), new Compiler.CorLibModuleProvider(), null);
            var typeDesc = module.Create(typeDef);
            var computedFieldLayout = metadataFieldLayoutAlgorithm.ComputeInstanceLayout(typeDesc as DefType);

            // Byte count
            // struct   MyStruct0   12
            // bool     MyBool      1 + 3 for int alignment
            // -----------------------
            //                      16

            var instanceByteCountUnaligned = computedFieldLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedFieldLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            Assert.That(instanceByteCount.AsInt, Is.EqualTo(16));

            foreach (var f in typeDef.Fields)
            {
                if (f.IsStatic)
                    continue;

                var fieldOffset = GetFieldOffset(computedFieldLayout.Offsets, f.Name);

                switch (f.Name)
                {
                    case "MyStruct0":
                        Assert.That(fieldOffset, Is.EqualTo(0));
                        break;
                    case "MyBool":
                        Assert.That(fieldOffset, Is.EqualTo(12));
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