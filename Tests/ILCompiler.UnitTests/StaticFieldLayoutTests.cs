using System.IO;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using ILCompiler.IL;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using NUnit.Framework;

namespace ILCompiler.UnitTests
{
    [TestFixture]
    public class StaticFieldLayoutTests
    {
        private ModuleDefMD _testModule;

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..");

        [SetUp]
        public void Setup()
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();

            var directoryInfo = new DirectoryInfo(Assembly.GetExecutingAssembly().Location);
            var configAndTargetFramework = $"{directoryInfo.Parent.Parent.Name}{Path.DirectorySeparatorChar}{directoryInfo.Parent.Name}";

            var currentType = MethodBase.GetCurrentMethod().DeclaringType;
            var assemblyConfigurationAttribute = currentType.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\System.Private.CoreLib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);
            ((AssemblyResolver)modCtx.AssemblyResolver).AddToCache(corlibModule);

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibModule.Assembly.ToAssemblyRef()
            };
            string inputFilePath = Path.Combine(SolutionPath, $@".\Tests\CoreTestAssembly\bin\{configAndTargetFramework}\CoreTestAssembly.dll");
            _testModule = ModuleDefMD.Load(inputFilePath, options);
        }

        private static DnlibModule CreateModule()
        {
            return new DnlibModule(
                new TypeSystemContext(new Configuration()),
                new Compiler.CorLibModuleProvider(),
                new RTILProvider());
        }

        [Test]
        public void TestNoPointers()
        {
            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/NoPointers", false);

            //var target = new TargetDetails(TargetArchitecture.Z80);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "int1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "byte1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(4));
                        break;
                    case "char1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(6));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestStillNoPointers()
        {
            //
            // Test that static offsets ignore instance fields preceding them
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/StillNoPointers", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "bool1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestClassNoPointers()
        {
            //
            // Ensure classes behave the same as structs when containing statics
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/ClassNoPointers", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "int1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "byte1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(4));
                        break;
                    case "char1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(6));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestHasPointers()
        {
            //
            // Test a struct containing static types with pointers
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/HasPointers", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "string1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "class1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(2));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestMixPointersAndNonPointers()
        {
            //
            // Test that static fields with GC pointers get separate offsets from non-GC fields
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/MixPointersAndNonPointers", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "string1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "int1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "class1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(2));
                        break;
                    case "int2":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(4));
                        break;
                    case "string2":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(4));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestEnsureInheritanceResetsStaticOffsets()
        {
            //
            // Test that when inheriting a class with static fields, the derived slice's static fields
            // are again offset from 0
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/EnsureInheritanceResetsStaticOffsets", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "int3":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "string3":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestLiteralFieldsDontAffectLayout()
        {
            //
            // Test that literal fields are not laid out.
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/LiteralFieldsDontAffectLayout", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            Assert.That(typeDesc.GetFields().Count(), Is.EqualTo(4));

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "IntConstant":
                    case "StringConstant":
                        Assert.That(field.IsStatic, Is.True);
                        Assert.That(field.IsLiteral, Is.True);
                        break;
                    case "Int1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    case "String1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [Test]
        public void TestStaticSelfRef()
        {
            //
            // Test that we can load a struct which has a static field referencing itself without
            // going into an infinite loop
            //

            TypeDef typeDef = _testModule.Find("CoreTestAssembly.StaticFieldLayout/StaticSelfRef", false);

            DnlibModule module = CreateModule();
            TypeDesc typeDesc = module.Create(typeDef);

            foreach (FieldDesc field in typeDesc.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                switch (field.Name)
                {
                    case "selfRef1":
                        Assert.That(field.Offset.AsInt, Is.EqualTo(0));
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}
